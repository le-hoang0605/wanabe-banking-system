namespace Accounts.Features.ChangeAccountStatus;

public interface IChangeAccountStatusService
{
    Task<(bool IsSuccess, string? ErrorMessage, ChangeAccountStatusResponseDto? Data)> ExecuteAsync(string accountNumber, ChangeAccountStatusRequestDto dto);
}