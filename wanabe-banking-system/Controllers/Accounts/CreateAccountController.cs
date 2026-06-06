using Accounts.Features.CreateAccount;
using Microsoft.AspNetCore.Mvc;

namespace wanabe_banking_system.Controllers.Accounts;

[ApiController]
public class CreateAccountController : ControllerBase
{
    private readonly ICreateAccountService _service;

    public CreateAccountController(ICreateAccountService service)
    {
        _service = service;
    }

    [HttpPost("api/v1/accounts/create")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequestDto dto)
    {
        var result = await _service.ExecuteAsync(dto);
        
        return Created($"api/v1/accounts/{result.AccountNumber}", result);
    }
}