using MediatR;
using BankDemo.Domain.Account;

namespace BankDemo.Application.Queries
{
    public class GetAccountByIdQuery(Guid id) : IRequest<Account>
    {
        public Guid Id { get; } = id;
    }
}
