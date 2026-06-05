using Accounts.Features.PostLedgerEntry;
using Microsoft.AspNetCore.Mvc;

namespace wanabe_banking_system.Controllers.Accounts;

[ApiController]
public class PostLedgerEntryController : ControllerBase
{
    private readonly IPostLedgerEntryService _service;

    public PostLedgerEntryController(IPostLedgerEntryService service)
    {
        _service = service;
    }

    [HttpPost("api/v1/accounts/post-entry")]
    public async Task<IActionResult> PostLedgerEntry([FromBody] AccountPostingRequestDto dto)
    {
        var result = await _service.ExecuteAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }
}