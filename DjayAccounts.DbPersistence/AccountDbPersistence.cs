using DjayAccounts.DbPersistence.Contexts;
using DjayAccounts.DbPersistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public async Task<CustomerModel> CreateCustomerAsync(Guid customerId, string fullName)
    {
        using var context = new AccountDbContext(_options);

        var customer = new Customer
        {
            CustomerId = customerId,
            FullName = fullName,
            CreatedAt = DateTime.UtcNow
        };

        context.Customers.Add(customer);
        await context.SaveChangesAsync();

        return _mapper.Map<CustomerModel>(customer);
    }

    /// <summary>
    /// Creates a new account for a given customer.
    /// </summary>
    public async Task<AccountModel> CreateAccountAsync(Guid accountId, Guid customerId, string accountType, string currency, decimal initialBalance)
    {
        using var context = new AccountDbContext(_options);

        var account = new Account
        {
            AccountId = accountId,
            CustomerId = customerId,
            AccountType = accountType,
            Currency = currency,
            Balance = initialBalance,
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        return MapAccountToBusiness(account);
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

        return account == null ? null : MapAccountToBusiness(account);
    }

    /// <summary>
    /// Freezes an account by setting its status to FROZEN.
    /// </summary>
    public async Task<AccountModel?> FreezeAccountAsync(Guid accountId)
    {
        using var context = new AccountDbContext(_options);

        var account = await context.Accounts.FirstOrDefaultAsync(a => a.AccountId == accountId);
        if (account == null) return null;

        account.Status = "FROZEN";
        account.FrozenAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return MapAccountToBusiness(account);
    }

    /// <summary>
    /// Maps an EF Account entity to the appropriate business model.
    /// </summary>
    private AccountModel MapAccountToBusiness(Account account)
    {
        return account.AccountType.ToUpper() switch
        {
            "SAVINGS" => _mapper.Map<SavingsAccountModel>(account),
            "CURRENT" => _mapper.Map<CurrentAccountModel>(account),
            _ => _mapper.Map<AccountModel>(account)
        };
    }
}