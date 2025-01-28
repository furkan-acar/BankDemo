using MediatR;
using BankDemo.Domain.Account;

namespace BankDemo.Application.Commands
{
    public class DeleteAccountCommand(Guid id) : IRequest<Account>
    {
        public Guid Id { get; set; } = id;
    }
}