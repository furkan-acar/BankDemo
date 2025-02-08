using System.ComponentModel.DataAnnotations;
using BankDemo.SharedKernel;

namespace BankDemo.Domain.Account;

public class Account(string name, decimal balance) : IAggregateRoot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = name;
    public decimal Balance { get; set; } = balance;

    [Timestamp]
    public uint Version { get; set; }

    public void Debit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        if (Balance < amount)
            throw new InvalidOperationException("Insufficient funds.");

        Balance -= amount;
    }

    public void Credit(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        Balance += amount;
    }
}
