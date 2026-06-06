using Microsoft.AspNetCore.Mvc;
using Transactions.Features.GetPaymentOrders;

namespace wanabe_banking_system.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Tags("Transactions")]
    public class GetPaymentOrdersController : ControllerBase
    {
        private readonly IGetPaymentOrdersHandler _handler;

        public GetPaymentOrdersController(IGetPaymentOrdersHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("history/{accountId:guid}")]
        public async Task<IActionResult> GetHistory(Guid accountId)
        {
            var history = await _handler.HandleAsync(accountId);
            return Ok(history);
        }
    }
}