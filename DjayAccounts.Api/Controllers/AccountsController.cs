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
    /// Creates a new current account for an existing customer.
    /// </summary>
    /// <param name="request">Account creation request.</param>
    /// <returns>Service result code.</returns>
    [HttpPost("current")]
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
    [HttpPost("savings")]
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
    [HttpPost("{accountId:guid}/freeze")]
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
    [HttpPost("{accountId:guid}/unfreeze")]
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
    [HttpGet("{accountId:guid}")]
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
    [HttpGet("search")]
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
