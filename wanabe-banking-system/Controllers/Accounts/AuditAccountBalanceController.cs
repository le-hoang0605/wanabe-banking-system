using Accounts.Features.AuditAccountBalance;
using Microsoft.AspNetCore.Mvc;

namespace wanabe_banking_system.Controllers.Accounts;

[ApiController]
public class AuditAccountBalanceController : ControllerBase
{
    private readonly IAuditAccountBalanceService _service;

    public AuditAccountBalanceController(IAuditAccountBalanceService service)
    {
        _service = service;
    }

    [HttpGet("api/v1/accounts/{accountNumber}/audit")]
    public async Task<IActionResult> AuditAccountBalance([FromRoute] string accountNumber)
    {
        var result = await _service.ExecuteAsync(accountNumber);

        if (result == null)
        {
            return NotFound(new { message = "Account not found to audit." });
        }

        return Ok(result);
    }
}