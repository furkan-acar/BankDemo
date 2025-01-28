using BankDemo.Domain.Account;
using MediatR;


namespace BankDemo.Application.Commands
{
    public class DepositCommand(Guid id, decimal amount) : IRequest<Account>
    {
        public Guid Id { get; } = id;
        public decimal Amount { get; } = amount;
    }
}