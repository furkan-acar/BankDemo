using MediatR;
using BankDemo.Domain.Account;

namespace BankDemo.Application.Commands;

public class TransferCommand : IRequest<Account?>
{
    public Guid SourceAccountId { get; set; }
    public Guid DestinationAccountId { get; set; }
    public decimal Amount { get; set; }
}
