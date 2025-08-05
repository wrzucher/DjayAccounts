using System.ComponentModel.DataAnnotations;

namespace DjayAccounts.Api.Models;

/// <summary>
/// Represents a savings account in the API layer.
/// </summary>
public class SavingsAccountDto : AccountDto
{
    /// <summary>
    /// Gets or sets a value that defines the interest rate for this savings account.
    /// </summary>
    [Required]
    public decimal InterestRate { get; set; }
}