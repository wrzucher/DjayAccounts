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

    /// <summary>
    /// Initializes a new instance of GameManager.
    /// </summary>
    /// <param name="persistence">Persistence layer for database access.</param>
    public AccountManager(AccountDbPersistence persistence)
    {
        this._persistence = persistence;
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
            accountId,
            customerId,
            AccountType.Current,
            currency,
            initialBalance,
            overdraftLimit);

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
    /// Retrieves account details by account ID.
    /// </summary>
    public async Task<AccountModel?> GetAccountAsync(Guid accountId)
    {
        return await _persistence.GetAccountAsync(accountId);
    }

    /// <summary>
    /// Retrieves all accounts by customer ID.
    /// </summary>
    public async Task<IEnumerable<AccountModel>> GetAccountsByCustomerIdAsync(Guid customerId)
    {
        return await _persistence.GetAccountsByCustomerIdAsync(customerId);
    }

    /// <summary>
    /// Retrieves customer details by customer ID.
    /// </summary>
    public async Task<CustomerModel?> GetCustomerByIdAsync(Guid customerId)
    {
        return await _persistence.GetCustomerByIdAsync(customerId);
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
        return await _persistence.GetCustomersAsync(firstNameFilter, lastNameFilter, page, pageSize);
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
        return await _persistence.GetAccountsAsync(
            page,
            pageSize,
            customerId,
            accountType,
            currency,
            status,
            minBalance,
            maxBalance,
            createdAfter,
            createdBefore,
            isFrozen);
    }
}
