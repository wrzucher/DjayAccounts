namespace DjayAccounts.EntityFramework.Entities;

/// <summary>
/// Represents a bank account owned by a customer.
/// Accounts can be of different types (e.g., Current, Savings).
/// </summary>
public class Account
{
    /// <summary>
    /// Gets or sets a value that uniquely identifies the account.
    /// Provided externally by the client to support idempotency.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets a value that links the account to its owner (customer).
    /// Must match an existing CustomerId.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets a value that defines the type of account (e.g., CURRENT, SAVINGS).
    /// </summary>
    public required string AccountType { get; set; }

    /// <summary>
    /// Gets or sets a value that defines the currency of the account balance.
    /// Should follow ISO 4217 standard (e.g., USD, EUR).
    /// </summary>
    public required string Currency { get; set; }

    /// <summary>
    /// Gets or sets a value that represents the current monetary balance of the account.
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the current status of the account.
    /// Possible values include ACTIVE, FROZEN, CLOSED.
    /// </summary>
    public required string Status { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates when the account was created.
    /// Stored in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates when the account was frozen.
    /// Null if the account is not frozen.
    /// </summary>
    public DateTime? FrozenAt { get; set; }

    /// <summary>
    /// Gets or sets a value that defines the overdraft limit for this current account.
    /// </summary>
    public decimal? OverdraftLimit { get; set; }

    /// <summary>
    /// Gets or sets a value that defines the interest rate for this savings account.
    /// </summary>
    public decimal? InterestRate { get; set; }
}