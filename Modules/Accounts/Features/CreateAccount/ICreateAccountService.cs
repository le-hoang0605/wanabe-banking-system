namespace Accounts.Features.CreateAccount;

public interface ICreateAccountService
{
    Task<CreateAccountResponseDto> ExecuteAsync(CreateAccountRequestDto dto);
}