namespace Accounts.Features.AuditAccountBalance;

public record AuditAccountBalanceResultDto(
    string AccountNumber,
    double CachedBalanceInTable,
    double RealCalculatedBalanceFromLedger,
    int TotalTransactionEntries,
    string DataIntegrity
);