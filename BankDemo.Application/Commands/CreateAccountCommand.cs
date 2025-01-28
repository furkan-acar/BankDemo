using BankDemo.Domain.Account;
using MediatR;

namespace BankDemo.Application.Commands
{
    public class CreateAccountCommand : IRequest<Account>
    {
        public required string Name { get; set; }
        public decimal Balance { get; set; }
    }
}