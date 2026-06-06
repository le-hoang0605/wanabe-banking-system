using Accounts.Features.ChangeAccountStatus;
using Microsoft.AspNetCore.Mvc;

namespace wanabe_banking_system.Controllers.Accounts;

[ApiController]
public class ChangeAccountStatusController : ControllerBase
{
    private readonly IChangeAccountStatusService _service;

    public ChangeAccountStatusController(IChangeAccountStatusService service)
    {
        _service = service;
    }

    [HttpPut("api/v1/accounts/{accountNumber}/change-status")]
    public async Task<IActionResult> ChangeStatus([FromRoute] string accountNumber, [FromBody] ChangeAccountStatusRequestDto dto)
    {
        var result = await _service.ExecuteAsync(accountNumber, dto);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }
}