using DjayAccounts.DbPersistence.ObjectModels;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DjayAccounts.Api.Models;

/// <summary>
/// Represents a bank account owned by a customer.
/// Accounts can be of different types (e.g., Current, Savings).
/// </summary>
[KnownType(typeof(CurrentAccountDto))]
[KnownType(typeof(SavingsAccountDto))]
public abstract class AccountDto
{
    /// <summary>
    /// Gets or sets a value that uniquely identifies the account.
    /// Provided externally by the client to support idempotency.
    /// </summary>
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets a value that links the account to its owner (customer).
    /// Must match an existing CustomerId.
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets a value that defines the type of account (e.g., CURRENT, SAVINGS).
    /// </summary>
    [Required]
    public AccountType AccountType { get; set; }

    /// <summary>
    /// Gets or sets a value that defines the currency of the account balance.
    /// Should follow ISO 4217 standard (e.g., USD, EUR).
    /// </summary>
    [Required]
    public string Currency { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value that represents the current monetary balance of the account.
    /// </summary>
    [Required]
    public decimal Balance { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates the current status of the account.
    /// </summary>
    [Required]
    public AccountStatus Status { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates when the account was created.
    /// Stored in UTC.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates when the account was frozen.
    /// Null if the account is not frozen.
    /// </summary>
    public DateTime? FrozenAt { get; set; }
}