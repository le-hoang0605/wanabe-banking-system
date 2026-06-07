using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.Features.GetAccountLedger;

namespace wanabe_banking_system.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Tags("Transactions")]
    [Authorize]
    public class GetAccountLedgerController : ControllerBase
    {
        private readonly IGetAccountLedgerHandler _handler;

        [HttpGet("ledger/{accountId:guid}")]
        public async Task<IActionResult> GetLedger(
             Guid accountId,
             [FromQuery] int pageNumber = 1,  // fisrt page
             [FromQuery] int pageSize = 10)   // ten line
        {
            //page check
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var ledger = await _handler.HandleAsync(accountId, pageNumber, pageSize);
            return Ok(ledger);
        }
    }
}