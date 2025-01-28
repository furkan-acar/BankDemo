using BankDemo.Domain.Account;
using MediatR;
using BankDemo.Application.Queries;
using BankDemo.SharedKernel;

namespace BankDemo.Application.Handlers;

public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, Account?>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByIdQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Account?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        return await _accountRepository.GetByIdAsync(request.Id);
    }
}
