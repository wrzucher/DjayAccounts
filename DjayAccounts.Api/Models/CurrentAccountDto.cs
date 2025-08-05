using System.ComponentModel.DataAnnotations;

namespace DjayAccounts.Api.Models;

/// <summary>
/// Represents a current account in the API layer.
/// </summary>
public class CurrentAccountDto : AccountDto
{
    /// <summary>
    /// Gets or sets a value that defines the overdraft limit for this current account.
    /// </summary>
    [Required]
    public decimal OverdraftLimit { get; set; }
}