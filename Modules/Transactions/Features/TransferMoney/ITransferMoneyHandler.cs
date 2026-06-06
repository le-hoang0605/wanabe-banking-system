using Transactions.DTOs;

namespace Transactions.Features.TransferMoney
{
    public interface ITransferMoneyHandler
    {
        Task<(bool IsValid, TransferResultDto? DuplicateResult, TransferSessionDto? Session)> PrepareTransferAsync(CreateTransferRequestDto request);
        Task ConfirmTransferAsync(Guid paymentId, bool isSuccess, string errorMessage);
    }
}
