using Transactions.DTOs;

namespace Transactions.Features.TransferMoney
{
    public interface ITransferMoneyHandler
    {
        //ths is for transfering money
        Task<(bool IsValid, TransferResultDto? DuplicateResult, TransferSessionDto? Session)> PrepareTransferAsync(TransferRequestWithKeyDto request);
        Task ConfirmTransferAsync(Guid paymentId, bool isSuccess, string errorMessage);

        //this is for depositing money
        Task<(bool IsValid, TransferResultDto? DuplicateResult, TransferSessionDto? Session)> PrepareDepositAsync(DepositRequestWithKeyDto request);
    }
}
