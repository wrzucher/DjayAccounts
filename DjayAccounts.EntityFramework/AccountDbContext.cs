using DjayAccounts.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace DjayAccounts.EntityFramework.Contexts;

/// <summary>
/// Represents the database context for the working with accounts.
/// </summary>
public class AccountDbContext : DbContext
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the collection of customers in the system.
    /// </summary>
    public DbSet<Customer> Customers { get; set; }

    /// <summary>
    /// Gets or sets the collection of accounts in the system.
    /// </summary>
    public DbSet<Account> Accounts { get; set; }

    /// <summary>
    /// Configures entity relationships, keys, indexes, and constraints.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Customers table
        modelBuilder.Entity<Customer>(entity =>
        {
            // Primary key
            entity.HasKey(c => c.CustomerId);

            entity.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(c => c.CreatedAt)
                .IsRequired();
        });

        // Configure Accounts table
        modelBuilder.Entity<Account>(entity =>
        {
            // Primary key
            entity.HasKey(a => a.AccountId);

            // Foreign key to Customers
            entity.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // AccountType is required
            entity.Property(a => a.AccountType)
                .IsRequired()
                .HasMaxLength(50);

            // Currency is required, 3-letter ISO code
            entity.Property(a => a.Currency)
                .IsRequired()
                .HasMaxLength(3);

            entity.Property(a => a.Balance)
                .IsRequired()
                .HasPrecision(2);

            entity.Property(a => a.Status)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(a => a.CreatedAt)
                .IsRequired();

            // Index for searching accounts by CustomerId
            entity.HasIndex(a => a.CustomerId);

            // Unique constraint: one AccountId per customer
            entity.HasIndex(a => new { a.CustomerId, a.AccountId })
                .IsUnique();
        });
    }
}
