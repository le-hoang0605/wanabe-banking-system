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
        public async Task<TransferResultDto> ExecuteWithdrawAsync(WithdrawRequestWithKeyDto request)
        {
            var (isValid, duplicateResult, session) = await _txHandler.PrepareWithdrawAsync(request);
            if (!isValid) return duplicateResult!;

            try
            {
                var debitResult = await _accountService.DebitAsync(session!.DebtorAccountId, session.Amount, session.PaymentId);

                if (!debitResult.IsSuccess)
                {
                    await _txHandler.ConfirmTransferAsync(session.PaymentId, isSuccess: false, debitResult.ErrorMessage);
                    return new TransferResultDto(false, debitResult.ErrorMessage, session.PaymentId, "Failed");
                }
                await _txHandler.ConfirmTransferAsync(session.PaymentId, isSuccess: true, "Success");
                return new TransferResultDto(true, "Withdraw successful!", session.PaymentId, "Executed");
            }
            catch (Exception ex)
            {
                await _txHandler.ConfirmTransferAsync(session!.PaymentId, isSuccess: false, ex.Message);
                return new TransferResultDto(false, $"System error: {ex.Message}", session!.PaymentId, "Failed");
            }
        }
        public async Task<TransferResultDto> ExecuteDepositAsync(DepositRequestWithKeyDto request)
        {
            var (isValid, duplicateResult, session) = await _txHandler.PrepareDepositAsync(request);
            if (!isValid) return duplicateResult!;

            try
            {
                var creditResult = await _accountService.CreditAsync(session!.CreditorAccountId, session.Amount, session.PaymentId);
                if (!creditResult.IsSuccess)
                {
                    await _txHandler.ConfirmTransferAsync(session.PaymentId, isSuccess: false, creditResult.ErrorMessage);
                    return new TransferResultDto(false, creditResult.ErrorMessage, session.PaymentId, "Failed");
                }
                await _txHandler.ConfirmTransferAsync(session.PaymentId, isSuccess: true, "Success");
                return new TransferResultDto(true, "Deposit successful!", session.PaymentId, "Executed");
            }
            catch (Exception ex)
            {
                await _txHandler.ConfirmTransferAsync(session!.PaymentId, isSuccess: false, ex.Message);
                return new TransferResultDto(false, $"System error: {ex.Message}", session!.PaymentId, "Failed");
            }
        }

        public async Task<TransferResultDto> ExecuteTransferAsync(TransferRequestWithKeyDto request)
        {
            var (isValid, duplicateResult, session) = await _txHandler.PrepareTransferAsync(request);
            if (!isValid) return duplicateResult!;
            try
            {
                var debitResult = await _accountService.DebitAsync(session!.DebtorAccountId, session.Amount, session.PaymentId);
                if (!debitResult.IsSuccess)
                {
                    await _txHandler.ConfirmTransferAsync(session.PaymentId, isSuccess: false, debitResult.ErrorMessage);
                    return new TransferResultDto(false, debitResult.ErrorMessage, session.PaymentId, "Failed");
                }

                var creditResult = await _accountService.CreditAsync(session.CreditorAccountId, session.Amount, session.PaymentId);
                if (!creditResult.IsSuccess)
                {
                    await _accountService.CreditAsync(session.DebtorAccountId, session.Amount, session.PaymentId);

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