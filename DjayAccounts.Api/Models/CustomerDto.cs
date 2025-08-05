using System.ComponentModel.DataAnnotations;

namespace DjayAccounts.Api.Models;

/// <summary>
/// Represents a customer in the banking system.
/// Each customer can own one or more accounts.
/// </summary>
public class CustomerDto
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
    [Required]
    public string FirstName { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value that contains the last name of the customer.
    /// </summary>
    [Required]
    public string LastName { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value that indicates when the customer record was created.
    /// Stored in UTC.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }
}