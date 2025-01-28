using MediatR;
using BankDemo.Domain.Account;

namespace BankDemo.Application.Queries
{
    public class GetAllAccountsQuery : IRequest<List<Account>>
    {
    }
}