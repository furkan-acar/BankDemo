namespace BankDemo.Domain.Account;

public class AccountConcurrencyException : Exception
{
    public Guid AccountId { get; }
    public uint ExpectedVersion { get; }
    public uint ActualVersion { get; }

    public AccountConcurrencyException(Guid accountId, uint expectedVersion, uint actualVersion, Exception? innerException = null)
        : base($"Concurrency conflict occurred for account {accountId}. Expected version {expectedVersion}, but found version {actualVersion}.", innerException)
    {
        AccountId = accountId;
        ExpectedVersion = expectedVersion;
        ActualVersion = actualVersion;
    }
}
