using System.ComponentModel.DataAnnotations;

namespace DjayAccounts.Api.Models;

/// <summary>
/// Request DTO for creating a current account.
/// </summary>
public class CreateCurrentAccountDto : CreateAccountDto
{
    /// <summary>
    /// Gets or sets a value that defines the overdraft limit for this current account.
    /// </summary>
    [Required, Range(0, double.MaxValue)]
    public decimal OverdraftLimit { get; set; }
}