using System.ComponentModel.DataAnnotations;

namespace DjayAccounts.Api.Models;

/// <summary>
/// Request DTO for creating a new customer.
/// </summary>
public class CreateCustomerDto
{
    /// <summary>
    /// Gets or sets a value that uniquely identifies the customer.
    /// Provided externally by the client.
    /// </summary>
    [Required]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets a value that contains the first name of the customer.
    /// </summary>
    [Required, StringLength(100)]
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value that contains the last name of the customer.
    /// </summary>
    [Required, StringLength(100)]
    public string LastName { get; set; } = default!;
}