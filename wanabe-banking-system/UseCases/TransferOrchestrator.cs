using Accounts.Features.PostLedgerEntry;
using Transactions.DTOs;
using Transactions.Features.TransferMoney;

namespace wanabe_banking_system.UseCases
{
    public class TransferOrchestrator
    {
        private readonly ITransferMoneyHandler _txHandler;
        private readonly IAccountService _accountService;

        public TransferOrchestrator(
            ITransferMoneyHandler txHandler,
            IAccountService accountService)
        {
            _txHandler = txHandler;
            _accountService = accountService;
        }

        public async Task<TransferResultDto> ExecuteTransferAsync(CreateTransferRequestDto request)
        {
            var (isValid, duplicateResult, session) = await _txHandler.PrepareTransferAsync(request);
            if (!isValid) return duplicateResult!;

            try
            {
                // 🔥 SỬA LỖI 1: Thêm .ToString() cho DebtorAccountId
                var debitResult = await _accountService.DebitAsync(session!.DebtorAccountId.ToString(), session.Amount, session.PaymentId);
                if (!debitResult.IsSuccess)
                {
                    await _txHandler.ConfirmTransferAsync(session.PaymentId, isSuccess: false, debitResult.ErrorMessage);
                    return new TransferResultDto(false, debitResult.ErrorMessage, session.PaymentId, "Failed");
                }

                var creditResult = await _accountService.CreditAsync(session.CreditorAccountId.ToString(), session.Amount, session.PaymentId);
                if (!creditResult.IsSuccess)
                {
                    await _accountService.CreditAsync(session.DebtorAccountId.ToString(), session.Amount, session.PaymentId);

                    await _txHandler.ConfirmTransferAsync(session.PaymentId, isSuccess: false, creditResult.ErrorMessage);
                    return new TransferResultDto(false, creditResult.ErrorMessage, session.PaymentId, "Failed");
                }

                await _txHandler.ConfirmTransferAsync(session.PaymentId, isSuccess: true, "Success");
                return new TransferResultDto(true, "Money transfer successful!", session.PaymentId, "Executed");
            }
            catch (Exception ex)
            {
                await _txHandler.ConfirmTransferAsync(session!.PaymentId, isSuccess: false, ex.Message);
                return new TransferResultDto(false, $"System error: {ex.Message}", session!.PaymentId, "Failed");
            }
        }
    }
}