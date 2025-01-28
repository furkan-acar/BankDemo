using BankDemo.Application.Commands;
using BankDemo.Domain.Account;
using BankDemo.SharedKernel;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace BankDemo.Application.Handlers;

public class TransferCommandHandler : IRequestHandler<TransferCommand, Account?>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<TransferCommandHandler> _logger;

    public TransferCommandHandler(IAccountRepository accountRepository, ILogger<TransferCommandHandler> logger)
    {
        _accountRepository = accountRepository;
        _logger = logger;
    }

    public async Task<Account?> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
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
        using (LogContext.PushProperty("AccountBalance", sourceAccount.Balance))
        using (LogContext.PushProperty("AccountId", sourceAccount.Id))
        using (LogContext.PushProperty("TransactionAmount", request.Amount))
        {
            _logger.LogInformation("Transaction completed for account {AccountId} with new balance {Balance}. Transaction amount: {request.Amount}",
                sourceAccount.Id, sourceAccount.Balance, request.Amount);
        }

        return sourceAccount;
    }
}
