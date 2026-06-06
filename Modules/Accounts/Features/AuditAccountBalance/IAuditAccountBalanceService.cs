namespace Accounts.Features.AuditAccountBalance;

public interface IAuditAccountBalanceService
{
    Task<AuditAccountBalanceResultDto?> ExecuteAsync(string accountNumber);
}