using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DjayAccounts.Api.Models;
using DjayAccounts.Core;
using DjayAccounts.DbPersistence.ObjectModels;
using System.ComponentModel.DataAnnotations;

namespace DjayAccounts.Api.Controllers;

/// <summary>
/// Provides endpoints for managing customers and their accounts.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AccountManager accountManager;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="accountManager">Business logic manager for accounts.</param>
    /// <param name="mapper">AutoMapper instance for DTO conversion.</param>
    public AccountController(AccountManager accountManager, IMapper mapper)
    {
        this.accountManager = accountManager;
        this.mapper = mapper;
    }

    /// <summary>
    /// Creates a new customer in the system.
    /// </summary>
    /// <param name="request">Customer creation request.</param>
    /// <returns>Service result code.</returns>
    [HttpPost("customers")]
    [ProducesResponseType(typeof(ServiceErrorCode), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceErrorCode>> CreateCustomer([FromBody] CreateCustomerDto request)
    {
        var result = await this.accountManager.CreateCustomerAsync(
            request.CustomerId,
            request.FirstName,
            request.LastName);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new current account for an existing customer.
    /// </summary>
    /// <param name="request">Account creation request.</param>
    /// <returns>Service result code.</returns>
    [HttpPost("accounts/current")]
    [ProducesResponseType(typeof(ServiceErrorCode), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceErrorCode>> CreateCurrentAccount([FromBody] CreateCurrentAccountDto request)
    {
        var result = await this.accountManager.CreateCurrentAccountAsync(
            request.AccountId,
            request.CustomerId,
            request.Currency,
            request.InitialBalance,
            request.OverdraftLimit);

        return Ok(result);
    }

    /// <summary>
    /// Creates a new savings account for an existing customer.
    /// </summary>
    /// <param name="request">Account creation request.</param>
    /// <returns>Service result code.</returns>
    [HttpPost("accounts/savings")]
    [ProducesResponseType(typeof(ServiceErrorCode), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceErrorCode>> CreateSavingsAccount([FromBody] CreateSavingsAccountDto request)
    {
        var result = await this.accountManager.CreateSavingsAccountAsync(
            request.AccountId,
            request.CustomerId,
            request.Currency,
            request.InitialBalance,
            request.InterestRate);

        return Ok(result);
    }

    /// <summary>
    /// Freezes an active account.
    /// </summary>
    /// <param name="accountId">Unique account identifier.</param>
    [HttpPost("accounts/{accountId:guid}/freeze")]
    [ProducesResponseType(typeof(ServiceErrorCode), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceErrorCode>> FreezeAccount([FromRoute] Guid accountId)
    {
        var result = await this.accountManager.FreezeAccountAsync(accountId);
        return Ok(result);
    }

    /// <summary>
    /// Unfreezes a frozen account.
    /// </summary>
    /// <param name="accountId">Unique account identifier.</param>
    [HttpPost("accounts/{accountId:guid}/unfreeze")]
    [ProducesResponseType(typeof(ServiceErrorCode), StatusCodes.Status200OK)]
    public async Task<ActionResult<ServiceErrorCode>> UnfreezeAccount([FromRoute] Guid accountId)
    {
        var result = await this.accountManager.UnfreezeAccountAsync(accountId);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves account details by account ID.
    /// </summary>
    /// <param name="accountId">Unique identifier for the account.</param>
    [HttpGet("accounts/{accountId:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountDto>> GetAccountById([FromRoute] Guid accountId)
    {
        var account = await this.accountManager.GetAccountAsync(accountId);
        if (account == null)
        {
            return NotFound();
        }

        var result = this.mapper.Map<AccountDto>(account);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all accounts owned by a specific customer.
    /// </summary>
    /// <param name="customerId">Unique identifier of the customer.</param>
    [HttpGet("customers/{customerId:guid}/accounts")]
    [ProducesResponseType(typeof(IEnumerable<AccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAccountsByCustomerId([FromRoute] Guid customerId)
    {
        var accounts = await this.accountManager.GetAccountsByCustomerIdAsync(customerId);
        var result = this.mapper.Map<IEnumerable<AccountDto>>(accounts);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves customer details by customer ID.
    /// </summary>
    /// <param name="customerId">Unique identifier of the customer.</param>
    [HttpGet("customers/{customerId:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(
        [FromRoute] Guid customerId)
    {
        var customer = await this.accountManager.GetCustomerByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound();
        }

        var dto = this.mapper.Map<CustomerDto>(customer);
        return Ok(dto);
    }

    /// <summary>
    /// Retrieves customers with pagination and optional filters.
    /// </summary>
    /// <param name="firstNameFilter">Optional filter for first name (case-insensitive contains).</param>
    /// <param name="lastNameFilter">Optional filter for last name (case-insensitive contains).</param>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    [HttpGet("customers")]
    [ProducesResponseType(typeof(PaginatedResult<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<CustomerDto>>> GetCustomers(
        [FromQuery] string? firstNameFilter,
        [FromQuery] string? lastNameFilter,
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20)
    {
        var customers = await this.accountManager.GetCustomersAsync(firstNameFilter, lastNameFilter, page, pageSize);
        var result = this.mapper.Map<PaginatedResult<CustomerDto>>(customers);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves accounts with pagination and optional filters.
    /// </summary>
    /// <param name="page">Page number (1-based).</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="customerId">Optional filter by customer ID.</param>
    /// <param name="accountType">Optional filter by account type.</param>
    /// <param name="currency">Optional filter by currency (ISO 4217 code).</param>
    /// <param name="status">Optional filter by account status.</param>
    /// <param name="minBalance">Optional minimum balance filter.</param>
    /// <param name="maxBalance">Optional maximum balance filter.</param>
    /// <param name="createdAfter">Optional filter for accounts created after this date (UTC).</param>
    /// <param name="createdBefore">Optional filter for accounts created before this date (UTC).</param>
    /// <param name="isFrozen">Optional filter for frozen accounts.</param>
    [HttpGet("accounts/search")]
    [ProducesResponseType(typeof(PaginatedResult<AccountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResult<AccountDto>>> GetAccountsWithFilters(
        [FromQuery, Range(1, int.MaxValue)] int page = 1,
        [FromQuery, Range(1, 100)] int pageSize = 20,
        [FromQuery] Guid? customerId = null,
        [FromQuery] AccountType? accountType = null,
        [FromQuery, StringLength(3, MinimumLength = 3)] string? currency = null,
        [FromQuery] AccountStatus? status = null,
        [FromQuery] decimal? minBalance = null,
        [FromQuery] decimal? maxBalance = null,
        [FromQuery] DateTime? createdAfter = null,
        [FromQuery] DateTime? createdBefore = null,
        [FromQuery] bool? isFrozen = null)
    {
        var accounts = await this.accountManager.GetAccountsAsync(
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
        var result = this.mapper.Map<PaginatedResult<AccountDto>>(accounts);
        return Ok(result);
    }
}
