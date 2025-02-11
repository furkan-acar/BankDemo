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
            throw new AccountNotFoundException(request.Id);
        }

        try
        {
            account.Update(request.Name, request.Balance, request.Version);
            await _accountRepository.UpdateAsync(account);
            return account;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            //to domain exception conversion
            throw new AccountConcurrencyException(
                request.Id,
                request.Version ?? 0,
                account.Version,
                ex);
        }
    }
}
