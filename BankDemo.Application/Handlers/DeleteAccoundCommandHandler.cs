using BankDemo.Domain.Account;
using MediatR;
using BankDemo.Application.Commands;
using BankDemo.SharedKernel;

namespace BankDemo.Application.Handlers;

public class DeleteAccoundCommandHandler : IRequestHandler<DeleteAccountCommand, Account?>
{
    private readonly IAccountRepository _accountRepository;

    public DeleteAccoundCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account?> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.Id);
        if (account != null)
        {
            await _accountRepository.DeleteAsync(account);
        }
        return account;
    }
}
