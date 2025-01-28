using BankDemo.Domain.Account;
using MediatR;
using BankDemo.Application.Commands;
using BankDemo.SharedKernel;

namespace BankDemo.Application.Handlers;

public class DepositCommandHandler : IRequestHandler<DepositCommand, Account?>
{
    private readonly IAccountRepository _accountRepository;

    public DepositCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account?> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.Id);
        if (account != null)
        {
            account.Credit(request.Amount);
            await _accountRepository.UpdateAsync(account);
        }
        return account;
    }
}
