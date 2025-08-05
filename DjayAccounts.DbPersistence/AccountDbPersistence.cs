using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DjayAccounts.DbPersistence.ObjectModels;
using DjayAccounts.EntityFramework.Contexts;
using DjayAccounts.EntityFramework.Entities;

namespace DjayAccounts.DbPersistence;

/// <summary>
/// Provides persistence operations for accounts using AccountDbContext.
/// This class abstracts database access and returns business models.
/// </summary>
public class AccountDbPersistence
{
    private readonly DbContextOptions<AccountDbContext> _options;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of AccountDbPersistence.
    /// </summary>
    /// <param name="options">Database context options.</param>
    /// <param name="mapper">AutoMapper instance for entity-to-business mapping.</param>
    public AccountDbPersistence(DbContextOptions<AccountDbContext> options, IMapper mapper)
    {
        _options = options;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new customer record.
    /// </summary>
    public async Task CreateCustomerAsync(Guid customerId, string firstName, string lastName)
    {
        using var context = new AccountDbContext(_options);

        var customer = new Customer
        {
            CustomerId = customerId,
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new current account for a given customer.
    /// </summary>
    public async Task CreateCurrentAccountAsync(
        Guid accountId,
        Guid customerId,
        AccountType accountType,
        string currency,
        decimal initialBalance,
        decimal overdraftLimit)
    {
        using var context = new AccountDbContext(_options);

        var account = new Account
        {
            AccountId = accountId,
            CustomerId = customerId,
            AccountType = accountType.ToString().ToUpperInvariant(),
            Currency = currency,
            Balance = initialBalance,
            OverdraftLimit = overdraftLimit,
            Status = AccountStatus.Active.ToString().ToUpperInvariant(),
            CreatedAt = DateTime.UtcNow
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new savings account for a given customer.
    /// </summary>
    public async Task CreateSavingsAccountAsync(
        Guid accountId,
        Guid customerId,
        AccountType accountType,
        string currency,
        decimal initialBalance,
        decimal interestRate)
    {
        using var context = new AccountDbContext(_options);

        var account = new Account
        {
            AccountId = accountId,
            CustomerId = customerId,
            AccountType = accountType.ToString().ToUpperInvariant(),
            Currency = currency,
            Balance = initialBalance,
            InterestRate = interestRate,
            Status = AccountStatus.Active.ToString().ToUpperInvariant(),
            CreatedAt = DateTime.UtcNow
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves account details by account ID.
    /// </summary>
    public async Task<AccountModel?> GetAccountAsync(Guid accountId)
    {
        using var context = new AccountDbContext(_options);

        var account = await context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AccountId == accountId);

        var result = _mapper.Map<AccountModel>(account);
        return result;
    }

    /// <summary>
    /// Freezes an account by account Id.
    /// </summary>
    public async Task FreezeAccountAsync(Guid accountId)
    {
        using var context = new AccountDbContext(_options);

        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.AccountId == accountId);
        if (account == null)
        {
            throw new InvalidOperationException($"Account with ID {accountId} doesn' found. Account can't be freezen");
        }

        account.Status = AccountStatus.Frozen.ToString().ToUpperInvariant();
        account.FrozenAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Unfreezes an account by account Id.
    /// </summary>
    public async Task UnfreezeAccountAsync(Guid accountId)
    {
        using var context = new AccountDbContext(_options);

        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.AccountId == accountId);
        if (account == null)
        {
            throw new InvalidOperationException($"Account with ID {accountId} doesn' found. Account can't be unfreezen");
        }

        account.Status = AccountStatus.Active.ToString().ToUpperInvariant();
        account.FrozenAt = null;

        await context.SaveChangesAsync();
    }
}