using System.ComponentModel.DataAnnotations;

namespace DjayAccounts.Api.Models;

/// <summary>
/// Request DTO for creating a new customer.
/// </summary>
public class CreateCustomerDto
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required, StringLength(100)]
    public string FirstName { get; set; } = default!;

    [Required, StringLength(100)]
    public string LastName { get; set; } = default!;
}