using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.Features.GetPaymentOrders;

namespace wanabe_banking_system.Controllers
{
    [ApiController]
    [Route("api/transactions")]
    [Tags("Transactions")]
    [Authorize]
    public class GetPaymentOrdersController : ControllerBase
    {
        private readonly IGetPaymentOrdersHandler _handler;

        public GetPaymentOrdersController(IGetPaymentOrdersHandler handler)
        {
            _handler = handler;
        }

        [HttpGet("history/{accountId:guid}")]
        public async Task<IActionResult> GetHistory(
            Guid accountId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var history = await _handler.HandleAsync(accountId, pageNumber, pageSize);
            return Ok(history);
        }
    }
}