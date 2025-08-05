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
    public async Task<CustomerModel> CreateCustomerAsync(Guid customerId, string firstName, string lastName)
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

        return _mapper.Map<CustomerModel>(customer);
    }

    /// <summary>
    /// Creates a new account for a given customer.
    /// </summary>
    public async Task<AccountModel> CreateAccountAsync(
        Guid accountId,
        Guid customerId,
        AccountType accountType,
        string currency,
        decimal initialBalance)
    {
        using var context = new AccountDbContext(_options);

        var account = new Account
        {
            AccountId = accountId,
            CustomerId = customerId,
            AccountType = accountType.ToString().ToUpperInvariant(),
            Currency = currency,
            Balance = initialBalance,
            Status = AccountStatus.Active.ToString().ToUpperInvariant(),
            CreatedAt = DateTime.UtcNow
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var result = _mapper.Map<AccountModel>(account);
        return result;
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
    /// Freezes an account by setting its status to FROZEN.
    /// </summary>
    public async Task<AccountModel?> FreezeAccountAsync(Guid accountId)
    {
        using var context = new AccountDbContext(_options);

        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.AccountId == accountId);
        if (account == null) return null;

        account.Status = AccountStatus.Frozen.ToString().ToUpperInvariant();
        account.FrozenAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        var result = _mapper.Map<AccountModel>(account);
        return result;
    }
}