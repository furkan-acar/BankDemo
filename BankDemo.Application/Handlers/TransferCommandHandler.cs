using BankDemo.Application.Commands;
using BankDemo.Domain.Account;
using BankDemo.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Transactions;

namespace BankDemo.Application.Handlers;

public class TransferCommandHandler : IRequestHandler<TransferCommand, Account?>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<TransferCommandHandler> _logger;
    private const int MaxRetries = 3;

    public TransferCommandHandler(IAccountRepository accountRepository, ILogger<TransferCommandHandler> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<Account?> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                return await ExecuteTransfer(request, attempt, cancellationToken);
            }
            catch (AccountConcurrencyException ex)
            {
                if (attempt == MaxRetries - 1)
                {
                    _logger.LogError(ex, "Transfer failed after {Attempts} attempts", MaxRetries);
                    throw;
                }

                var delay = TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt));
                _logger.LogWarning("Concurrency conflict, retrying in {Delay}ms. Attempt {Attempt}/{MaxRetries}", 
                    delay.TotalMilliseconds, attempt + 1, MaxRetries);
                
                await Task.Delay(delay, cancellationToken);
            }
        }

        throw new Exception("Unexpected code path"); // Should never get here. Will add custom exception type later.
    }

    private async Task<Account?> ExecuteTransfer(TransferCommand request, int attempt, CancellationToken cancellationToken)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        
        var sourceAccount = await _accountRepository.GetByIdAsync(request.SourceAccountId);
        var destinationAccount = await _accountRepository.GetByIdAsync(request.DestinationAccountId);

        if (sourceAccount == null || destinationAccount == null)
        {
            throw new InvalidOperationException("Source or destination account not found.");
        }

        if (sourceAccount.Balance < request.Amount)
        {
            throw new InvalidOperationException("Insufficient funds.");
        }

        sourceAccount.Debit(request.Amount);
        destinationAccount.Credit(request.Amount);

        await _accountRepository.UpdateAsync(sourceAccount);
        await _accountRepository.UpdateAsync(destinationAccount);

        scope.Complete();

        using (LogContext.PushProperty("AccountBalance", sourceAccount.Balance))
        using (LogContext.PushProperty("AccountId", sourceAccount.Id))
        using (LogContext.PushProperty("TransactionAmount", request.Amount))
        using (LogContext.PushProperty("DestinationId", destinationAccount.Id))
        using (LogContext.PushProperty("Attempt", attempt + 1))
        {
            _logger.LogInformation(
                "Transfer of {Amount} from {SourceId} to {DestId} completed successfully on attempt {Attempt}", 
                request.Amount, sourceAccount.Id, destinationAccount.Id, attempt + 1);
        }

        return sourceAccount;
    }
}
