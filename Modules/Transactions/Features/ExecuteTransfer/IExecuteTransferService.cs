namespace Transactions.Features.ExecuteTransfer;

public interface IExecuteTransferService
{
    Task<(bool IsSuccess, string ErrorMessage, ExecuteTransferResponseDto? Data)> ExecuteAsync(ExecuteTransferRequestDto dto);
}   