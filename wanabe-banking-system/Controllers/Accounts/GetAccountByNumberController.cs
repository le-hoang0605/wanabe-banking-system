using Accounts.Features.GetAccountByNumber;
using Microsoft.AspNetCore.Mvc;

namespace wanabe_banking_system.Controllers.Accounts;

[ApiController]
[Tags("Accounts")]
public class GetAccountByNumberController : ControllerBase
{
    private readonly IGetAccountByNumberService _service;

    public GetAccountByNumberController(IGetAccountByNumberService service)
    {
        _service = service;
    }

    [HttpGet("api/v1/accounts/{accountNumber}")]
    public async Task<IActionResult> GetByAccountNumber([FromRoute] string accountNumber)
    {
        var result = await _service.ExecuteAsync(accountNumber);

        if (result == null)
        {
            return NotFound(new { message = $"Account number {accountNumber} not found." });
        }

        return Ok(result);
    }
}