namespace DjayAccounts.EntityFramework.Entities;

using System;

/// <summary>
/// Represents a customer in the banking system.
/// Each customer can own one or more accounts.
/// </summary>
public class Customer
{
    /// <summary>
    /// Gets or sets a value that uniquely identifies the customer.
    /// Provided externally by the client.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets a value that contains the first name of the customer.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Gets or sets a value that contains the last name of the customer.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Gets or sets a value that indicates when the customer record was created.
    /// Stored in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
