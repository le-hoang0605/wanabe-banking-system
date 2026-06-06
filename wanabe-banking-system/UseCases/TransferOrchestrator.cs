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

                var debitResult = await _accountService.DebitAsync(session!.DebtorAccountId, session.Amount, session.PaymentId);
                if (!debitResult.IsSuccess)
                {
                    // Đã sửa: Truyền đúng PaymentId và ErrorMessage theo thiết kế mới
                    await _txHandler.ConfirmTransferAsync(session.PaymentId, isSuccess: false, debitResult.ErrorMessage);
                    return new TransferResultDto(false, debitResult.ErrorMessage, session.PaymentId, "Failed");
                }

                // BƯỚC 2: Gọi Accounts cộng tiền tài khoản nhận
                var creditResult = await _accountService.CreditAsync(session.CreditorAccountId, session.Amount, session.PaymentId);
                if (!creditResult.IsSuccess)
                {
                    // Hoàn tác (Compensate): Cộng trả lại tiền cho người gửi nếu người nhận bị lỗi
                    await _accountService.CreditAsync(session.DebtorAccountId, session.Amount, session.PaymentId);

                    await _txHandler.ConfirmTransferAsync(session.PaymentId, isSuccess: false, creditResult.ErrorMessage);
                    return new TransferResultDto(false, creditResult.ErrorMessage, session.PaymentId, "Failed");
                }

                // BƯỚC 3: Chốt đơn hàng thành công viên mãn
                await _txHandler.ConfirmTransferAsync(session.PaymentId, isSuccess: true, "Success");
                return new TransferResultDto(true, "Money transfer successful!", session.PaymentId, "Executed");
            }
            catch (Exception ex)
            {
                // Phòng hờ hệ thống sập nguồn bất ngờ
                await _txHandler.ConfirmTransferAsync(session!.PaymentId, isSuccess: false, ex.Message);
                return new TransferResultDto(false, $"System error: {ex.Message}", session!.PaymentId, "Failed");
            }
        }
    }
}