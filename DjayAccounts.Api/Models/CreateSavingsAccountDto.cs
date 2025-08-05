using System.ComponentModel.DataAnnotations;

namespace DjayAccounts.Api.Models;

/// <summary>
/// Request DTO for creating a savings account.
/// </summary>
public class CreateSavingsAccountDto : CreateAccountDto
{
    /// <summary>
    /// Gets or sets a value that defines the interest rate for this savings account.
    /// </summary>
    [Required, Range(0.01, double.MaxValue)]
    public decimal InterestRate { get; set; }
}