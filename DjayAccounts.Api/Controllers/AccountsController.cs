using Microsoft.AspNetCore.Mvc;

namespace DjayAccounts.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(ILogger<AccountsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetAccounts")]
    public IEnumerable<object> Get()
    {
        throw new NotImplementedException();
    }
}
