using BankDemo.Application.Commands;
using MediatR;
using BankDemo.Domain.Account;
using BankDemo.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace BankDemo.Application.Handlers;

public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, Account?>
{
    private readonly IAccountRepository _accountRepository;

    public UpdateAccountCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account?> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.Id);
        if (account == null)
        {
            throw new Exception("Account not found");
        }

        if (account.Version != request.Version)
        {
            throw new DbUpdateConcurrencyException("The account has been modified by another user.");
        }

        account.Name = request.Name;
        account.Balance = request.Balance;
        await _accountRepository.UpdateAsync(account);
        return account;
    }
}
