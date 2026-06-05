namespace Accounts.Features.PostLedgerEntry;

public interface IPostLedgerEntryService
{
    Task<(bool IsSuccess, string ErrorMessage, AccountPostingResponseDto? Data)> ExecuteAsync(AccountPostingRequestDto dto);
}