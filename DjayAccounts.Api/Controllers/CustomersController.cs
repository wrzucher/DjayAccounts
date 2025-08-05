using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DjayAccounts.Api.Models;
using DjayAccounts.Core;
using DjayAccounts.DbPersistence.ObjectModels;
using System.ComponentModel.DataAnnotations;

namespace DjayAccounts.Api.Controllers;

/// <summary>
/// Provides endpoints for managing customers.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AccountManager accountManager;
    private readonly IMapper mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="accountManager">Business logic manager for accounts.</param>
    /// <param name="mapper">AutoMapper instance for DTO conversion.</param>
    public CustomersController(AccountManager accountManager, IMapper mapper)
    {
        this.accountManager = accountManager;
        this.mapper = mapper;
    }

    /// <summary>
    /// Creates a new customer in the system.
    /// </summary>
    /// <param name="request">Customer creation request.</param>
    /// <returns>Service result code.</returns>
    [HttpPost()]
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
    /// Retrieves all accounts owned by a specific customer.
    /// </summary>
    /// <param name="customerId">Unique identifier of the customer.</param>
    [HttpGet("{customerId:guid}/accounts")]
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
    [HttpGet("{customerId:guid}")]
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
    [HttpGet()]
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
}
