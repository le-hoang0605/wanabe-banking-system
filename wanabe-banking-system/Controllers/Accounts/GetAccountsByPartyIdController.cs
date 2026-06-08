using Accounts.Features.GetAccountsByPartyId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace wanabe_banking_system.Controllers.Accounts;

[ApiController]
[Tags("Accounts")]
public class GetAccountsByPartyIdController : ControllerBase
{
    private readonly IGetAccountsByPartyIdService _service;

    public GetAccountsByPartyIdController(IGetAccountsByPartyIdService service)
    {
        _service = service;
    }

    [HttpGet("api/accounts/party/{partyId:guid}")]
    public async Task<IActionResult> GetByPartyId([FromRoute] Guid partyId)
    {
        var result = await _service.ExecuteAsync(partyId);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("api/accounts/my-accounts")]
    public async Task<ActionResult> GetMyAccounts()
    {
        var partyIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    
        if (string.IsNullOrEmpty(partyIdClaim)) return Unauthorized("Token không hợp lệ.");

        var partyId = Guid.Parse(partyIdClaim);
        
        var accounts = await _service.ExecuteAsync(partyId);
    
        return Ok(accounts);
    }
}