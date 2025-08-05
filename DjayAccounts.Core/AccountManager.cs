using DjayAccounts.DbPersistence;
using DjayAccounts.DbPersistence.ObjectModels;
using DjayAccounts.EntityFramework.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DjayAccounts.Core;

/// <summary>
/// Provides business logic operations for managing accounts and customers.
/// Wraps persistence and enforces business rules.
/// </summary>
public class AccountManager
{
    private readonly AccountDbPersistence _persistence;
    private readonly DbContextOptions<AccountDbContext> _options;

    /// <summary>
    /// Initializes a new instance of GameManager.
    /// </summary>
    /// <param name="persistence">Persistence layer for database access.</param>
    /// <param name="options">Database context options.</param>
    public AccountManager(AccountDbPersistence persistence, DbContextOptions<AccountDbContext> options)
    {
        this._persistence = persistence;
        this._options = options;
    }

    /// <summary>
    /// Creates a new customer if not exists.
    /// </summary>
    /// <param name="customerId">Unique identifier for the customer.</param>
    /// <param name="firstName">Customer's first name.</param>
    /// <param name="lastName">Customer's last name.</param>
    public async Task<ServiceErrorCode> CreateCustomerAsync(Guid customerId, string firstName, string lastName)
    {
        var existing = await this._persistence.GetCustomerByIdAsync(customerId);
        if (existing != null)
        {
            return ServiceErrorCode.CustomerAlreadyExists;
        }

        await this._persistence.CreateCustomerAsync(customerId, firstName, lastName);
        return ServiceErrorCode.Ok;
    }

    /// <summary>
    /// Creates a new current account for an existing customer.
    /// </summary>
    /// <param name="accountId">Unique identifier for the account.</param>
    /// <param name="customerId">Identifier of the customer who owns the account.</param>
    /// <param name="currency">Account currency (e.g., USD, EUR).</param>
    /// <param name="initialBalance">Initial deposit amount.</param>
    /// <param name="overdraftLimit">Maximum overdraft limit allowed.</param>
    public async Task<ServiceErrorCode> CreateCurrentAccountAsync(
        Guid accountId,
        Guid customerId,
        string currency,
        decimal initialBalance,
        decimal overdraftLimit)
    {
        var customer = await this._persistence.GetCustomerByIdAsync(customerId);
        if (customer == null)
        {
            return ServiceErrorCode.CustomerNotFound;
        }

        var existingAccount = await this._persistence.GetAccountAsync(accountId);
        if (existingAccount != null)
        {
            return ServiceErrorCode.AccountAlreadyExists;
        }

        if (overdraftLimit < 0)
        {
            return ServiceErrorCode.ValidationFailed;
        }

        await this._persistence.CreateCurrentAccountAsync(
            accountId, customerId, AccountType.Current, currency, initialBalance, overdraftLimit);

        return ServiceErrorCode.Ok;
    }

    /// <summary>
    /// Creates a new savings account for an existing customer.
    /// </summary>
    /// <param name="accountId">Unique identifier for the account.</param>
    /// <param name="customerId">Identifier of the customer who owns the account.</param>
    /// <param name="currency">Account currency (e.g., USD, EUR).</param>
    /// <param name="initialBalance">Initial deposit amount.</param>
    /// <param name="interestRate">Annual interest rate for the savings account.</param>
    public async Task<ServiceErrorCode> CreateSavingsAccountAsync(
        Guid accountId,
        Guid customerId,
        string currency,
        decimal initialBalance,
        decimal interestRate)
    {
        var customer = await this._persistence.GetCustomerByIdAsync(customerId);
        if (customer == null)
        {
            return ServiceErrorCode.CustomerNotFound;
        }

        var existingAccount = await this._persistence.GetAccountAsync(accountId);
        if (existingAccount != null)
        {
            return ServiceErrorCode.AccountAlreadyExists;
        }

        if (interestRate <= 0)
        {
            return ServiceErrorCode.ValidationFailed;
        }

        await this._persistence.CreateSavingsAccountAsync(
            accountId, customerId, AccountType.Savings, currency, initialBalance, interestRate);

        return ServiceErrorCode.Ok;
    }

    /// <summary>
    /// Freezes an active account.
    /// </summary>
    /// <param name="accountId">Unique identifier for the account.</param>
    public async Task<ServiceErrorCode> FreezeAccountAsync(Guid accountId)
    {
        var account = await this._persistence.GetAccountAsync(accountId);
        if (account == null)
        {
            return ServiceErrorCode.AccountNotFound;
        }

        if (account.Status == AccountStatus.Frozen)
        {
            return ServiceErrorCode.AccountAlreadyFrozen;
        }

        await this._persistence.FreezeAccountAsync(accountId);
        return ServiceErrorCode.Ok;
    }

    /// <summary>
    /// Unfreezes a frozen account.
    /// </summary>
    /// <param name="accountId">Unique identifier for the account.</param>
    public async Task<ServiceErrorCode> UnfreezeAccountAsync(Guid accountId)
    {
        var account = await this._persistence.GetAccountAsync(accountId);
        if (account == null)
        {
            return ServiceErrorCode.AccountNotFound;
        }

        if (account.Status == AccountStatus.Active)
        {
            return ServiceErrorCode.AccountNotFrozen;
        }

        await this._persistence.UnfreezeAccountAsync(accountId);
        return ServiceErrorCode.Ok;
    }

    /// <summary>
    /// Withdraws money from an account if balance is sufficient.
    /// </summary>
    /// <param name="accountId">Unique identifier for the account.</param>
    /// <param name="amount">Amount of money to withdraw.</param>
    public async Task<ServiceErrorCode> WithdrawAsync(Guid accountId, decimal amount)
    {
        var account = await this._persistence.GetAccountAsync(accountId);
        if (account == null)
        {
            return ServiceErrorCode.AccountNotFound;
        }

        if (account.Status != AccountStatus.Active)
        {
            return ServiceErrorCode.AccountClosed;
        }

        if (account.Balance < amount)
        {
            return ServiceErrorCode.InsufficientFunds;
        }

        using var context = new AccountDbContext(this._options);
        var entity = await context.Accounts.FirstAsync(a => a.AccountId == accountId);
        entity.Balance -= amount;
        await context.SaveChangesAsync();

        return ServiceErrorCode.Ok;
    }

    /// <summary>
    /// Deposits money into an account.
    /// </summary>
    /// <param name="accountId">Unique identifier for the account.</param>
    /// <param name="amount">Amount of money to deposit.</param>
    public async Task<ServiceErrorCode> DepositAsync(Guid accountId, decimal amount)
    {
        var account = await this._persistence.GetAccountAsync(accountId);
        if (account == null)
        {
            return ServiceErrorCode.AccountNotFound;
        }

        if (account.Status != AccountStatus.Active)
        {
            return ServiceErrorCode.AccountClosed;
        }

        using var context = new AccountDbContext(this._options);
        var entity = await context.Accounts.FirstAsync(a => a.AccountId == accountId);
        entity.Balance += amount;
        await context.SaveChangesAsync();

        return ServiceErrorCode.Ok;
    }
}
