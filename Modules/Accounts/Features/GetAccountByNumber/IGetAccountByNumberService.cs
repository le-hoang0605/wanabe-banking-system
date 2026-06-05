namespace Accounts.Features.GetAccountByNumber;

public interface IGetAccountByNumberService
{
    Task<GetAccountByNumberResultDto?> ExecuteAsync(string accountNumber);
}