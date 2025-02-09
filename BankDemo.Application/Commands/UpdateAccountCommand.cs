using MediatR;
using BankDemo.Domain.Account;

namespace BankDemo.Application.Commands
{
    public class UpdateAccountCommand : IRequest<Account>
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public decimal Balance { get; set; }
        public uint Version { get; set; }
    }
}
