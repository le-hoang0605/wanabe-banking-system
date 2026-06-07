using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transactions.DTOs;
using wanabe_banking_system.UseCases;

namespace wanabe_banking_system.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    [Tags("Transactions")]
    [Authorize]
    public class TransferMoneyController : ControllerBase
    {
        private readonly TransferOrchestrator _orchestrator;

        public TransferMoneyController(TransferOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpPost("transfer")]
        [Authorize(Roles = "User")]
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
        [Authorize(Roles = "Admin,User")]
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
        [HttpPost("withdraw")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Withdraw([FromBody] CreateWithdrawRequestDto request, [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey)
        {
            var key = string.IsNullOrEmpty(idempotencyKey) ? Guid.NewGuid().ToString() : idempotencyKey;
            var internalRequest = new WithdrawRequestWithKeyDto(
                request.DebtorAccountId,
                request.Amount,
                key
            );
            var result = await _orchestrator.ExecuteWithdrawAsync(internalRequest);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}