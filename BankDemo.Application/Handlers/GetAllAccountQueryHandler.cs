using BankDemo.Domain.Account;
using MediatR;
using BankDemo.Application.Queries;
using BankDemo.SharedKernel;

namespace BankDemo.Application.Handlers;

public class GetAllAccountQueryHandler : IRequestHandler<GetAllAccountsQuery, List<Account>>
{
    private readonly IAccountRepository _accountRepository;

    public GetAllAccountQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public Task<List<Account>> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
    {
        return _accountRepository.GetAllAsync();
    }
}
