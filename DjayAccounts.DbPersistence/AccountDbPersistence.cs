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
    private const int MinSearchTermLength = 3;
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
            Currency = currency.ToUpperInvariant(),
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
            Currency = currency.ToUpperInvariant(),
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

        var result = _mapper.Map<AccountModel?>(account);
        return result;
    }

    /// <summary>
    /// Retrieves all accounts by customer ID.
    /// </summary>
    public async Task<IEnumerable<AccountModel>> GetAccountsByCustomerIdAsync(Guid customerId)
    {
        using var context = new AccountDbContext(_options);

        var accounts = await context.Accounts
            .AsNoTracking()
            .Where(a => a.CustomerId == customerId)
            .ToArrayAsync();

        var result = _mapper.Map<IEnumerable<AccountModel>>(accounts);
        return result;
    }

    /// <summary>
    /// Retrieves customer details by customer ID.
    /// </summary>
    public async Task<CustomerModel?> GetCustomerByIdAsync(Guid customerId)
    {
        using var context = new AccountDbContext(_options);

        var account = await context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.CustomerId == customerId);

        var result = _mapper.Map<CustomerModel?>(account);
        return result;
    }

    /// <summary>
    /// Retrieves customers details with pagination and filters.
    /// </summary>
    public async Task<PaginatedResult<CustomerModel>> GetCustomersAsync(
        string? firstNameFilter,
        string? lastNameFilter,
        int page,
        int pageSize)
    {
        using var context = new AccountDbContext(_options);

        var query = context.Customers.AsNoTracking().AsQueryable();

#warning it's better to have full-text index in the DB Server.
        if (!string.IsNullOrWhiteSpace(firstNameFilter)
            && firstNameFilter.Length > MinSearchTermLength)
        {
            query = query.Where(c => EF.Functions.Like(c.FirstName, $"%{firstNameFilter}%"));
        }

        if (!string.IsNullOrWhiteSpace(lastNameFilter)
            && lastNameFilter.Length > MinSearchTermLength)
        {
            query = query.Where(c => EF.Functions.Like(c.LastName, $"%{lastNameFilter}%"));
        }

        var totalCount = await query.CountAsync();

#warning it's better to skip this part if we do not have any filters
        var customers = await query
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = _mapper.Map<IEnumerable<CustomerModel>>(customers);

        return new PaginatedResult<CustomerModel>(page, pageSize, totalCount, result);
    }

    /// <summary>
    /// Retrieves accounts with pagination and optional filters.
    /// </summary>
    public async Task<PaginatedResult<AccountModel>> GetAccountsAsync(
        int page,
        int pageSize,
        Guid? customerId,
        AccountType? accountType,
        string? currency,
        AccountStatus? status,
        decimal? minBalance,
        decimal? maxBalance,
        DateTime? createdAfter,
        DateTime? createdBefore,
        bool? isFrozen)
    {
        using var context = new AccountDbContext(_options);

        var query = context.Accounts.AsNoTracking().AsQueryable();

        if (customerId.HasValue)
        {
            query = query.Where(a => a.CustomerId == customerId.Value);
        }

        if (accountType.HasValue)
        {
            // In the better world we want to have here something like this
            // a.Status.Equals(status.ToString(), StringComparison.InvariantCultureIgnoreCase));
            // but in fact it can be translated to SQL
            // so this equal should be ok
            query = query.Where(a => a.AccountType == accountType.Value.ToString().ToUpperInvariant());
        }

        if (!string.IsNullOrWhiteSpace(currency))
        {
            query = query.Where(a => a.Currency == currency.ToUpperInvariant());
        }

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value.ToString().ToUpperInvariant());
        }

        if (minBalance.HasValue)
        {
            query = query.Where(a => a.Balance >= minBalance.Value);
        }

        if (maxBalance.HasValue)
        {
            query = query.Where(a => a.Balance <= maxBalance.Value);
        }

        if (createdAfter.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= createdAfter.Value);
        }

        if (createdBefore.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= createdBefore.Value);
        }

        if (isFrozen.HasValue)
        {
            query = isFrozen.Value
                ? query.Where(a => a.FrozenAt != null)
                : query.Where(a => a.FrozenAt == null);
        }

        var totalCount = await query.CountAsync();

#warning Here should be check that we have at least 1 filter with indexed field
        // Type, status and currency should have indexes.
        // One of them should be in the filter because otherwise we will have full table scan

        var accounts = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = _mapper.Map<IEnumerable<AccountModel>>(accounts);
        return new PaginatedResult<AccountModel>(page, pageSize, totalCount, result);
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