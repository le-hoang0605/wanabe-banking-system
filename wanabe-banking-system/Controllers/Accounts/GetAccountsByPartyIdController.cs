using Accounts.Features.GetAccountsByPartyId;
using Microsoft.AspNetCore.Mvc;

namespace wanabe_banking_system.Controllers.Accounts;

[ApiController]
public class GetAccountsByPartyIdController : ControllerBase
{
    private readonly IGetAccountsByPartyIdService _service;

    public GetAccountsByPartyIdController(IGetAccountsByPartyIdService service)
    {
        _service = service;
    }

    [HttpGet("api/v1/accounts/party/{partyId:guid}")]
    public async Task<IActionResult> GetByPartyId([FromRoute] Guid partyId)
    {
        var result = await _service.ExecuteAsync(partyId);
        return Ok(result);
    }
}