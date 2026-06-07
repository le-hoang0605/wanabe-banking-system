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
        public async Task<IActionResult> Transfer([FromBody] CreateTransferRequestDto request, [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey)
        {
            //check idempotency key if we dont have we will creaate a new one
            var key = string.IsNullOrEmpty(idempotencyKey) ? Guid.NewGuid().ToString() : idempotencyKey;
            //we use DTO with key here
            var internalRequest = new TransferRequestWithKeyDto(
                request.DebtorAccountId,
                request.CreditorAccountId,
                request.Amount,
                key
            );
            var result = await _orchestrator.ExecuteTransferAsync(internalRequest);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] CreateDepositRequestDto request, [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey)
        {
            var key = string.IsNullOrEmpty(idempotencyKey) ? Guid.NewGuid().ToString() : idempotencyKey;
            var internalRequest = new DepositRequestWithKeyDto(
                request.CreditorAccountId,
                request.Amount,
                key
            );
            var result = await _orchestrator.ExecuteDepositAsync(internalRequest);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}