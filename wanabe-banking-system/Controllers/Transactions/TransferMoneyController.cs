using Microsoft.AspNetCore.Mvc;
using Transactions.DTOs;
using wanabe_banking_system.UseCases;

namespace wanabe_banking_system.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    [Tags("Transactions")]
    public class TransferMoneyController : ControllerBase
    {
        private readonly TransferOrchestrator _orchestrator;

        public TransferMoneyController(TransferOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] CreateTransferRequestDto request)
        {
            var result = await _orchestrator.ExecuteTransferAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}