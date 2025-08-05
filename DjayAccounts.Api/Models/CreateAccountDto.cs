using System.ComponentModel.DataAnnotations;

namespace DjayAccounts.Api.Models;

/// <summary>
/// Request DTO for creating an account.
/// </summary>
public abstract class CreateAccountDto
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
    /// Gets or sets a value that defines the currency of the account balance.
    /// Should follow ISO 4217 standard (e.g., USD, EUR).
    /// </summary>
    [Required, StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value that represents the initial monetary balance of the account.
    /// </summary>
    [Required, Range(0, double.MaxValue)]
    public decimal InitialBalance { get; set; }
}