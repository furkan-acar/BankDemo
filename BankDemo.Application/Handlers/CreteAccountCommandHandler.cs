using BankDemo.Domain.Account;
using MediatR;
using BankDemo.Application.Commands;
using BankDemo.SharedKernel;

namespace BankDemo.Application.Handlers;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Account>
{
    private readonly IAccountRepository _accountRepository;

    public CreateAccountCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account(request.Name, request.Balance);
        await _accountRepository.AddAsync(account);
        return account;
    }
}
