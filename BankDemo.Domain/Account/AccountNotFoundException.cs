namespace BankDemo.Domain.Account;

public class AccountNotFoundException : Exception
{
    public Guid AccountId { get; }

    public AccountNotFoundException(Guid accountId)
        : base($"Account with ID {accountId} was not found.")
    {
        AccountId = accountId;
    }
}
