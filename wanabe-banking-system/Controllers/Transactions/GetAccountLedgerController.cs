using Microsoft.AspNetCore.Mvc;
using Transactions.Features.GetAccountLedger;

namespace wanabe_banking_system.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Tags("Transactions")]
    public class GetAccountLedgerController : ControllerBase
    {
        private readonly IGetAccountLedgerHandler _handler;

        public GetAccountLedgerController(IGetAccountLedgerHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("ledger/{accountId:guid}")]
        public async Task<IActionResult> GetLedger(Guid accountId)
        {
            var ledger = await _handler.HandleAsync(accountId);
            return Ok(ledger);
        }
    }
}