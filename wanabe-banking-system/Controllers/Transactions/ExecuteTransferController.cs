using Microsoft.AspNetCore.Mvc;
using Transactions.Features.ExecuteTransfer;

namespace wanabe_banking_system.Controllers.Transactions;

[ApiController]
public class ExecuteTransferController : ControllerBase
{
    private readonly IExecuteTransferService _service;

    public ExecuteTransferController(IExecuteTransferService service)
    {
        _service = service;
    }

    [HttpPost("api/v1/transactions/transfer")]
    public async Task<IActionResult> ExecuteTransfer([FromBody] ExecuteTransferRequestDto dto)
    {
        var result = await _service.ExecuteAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }
}