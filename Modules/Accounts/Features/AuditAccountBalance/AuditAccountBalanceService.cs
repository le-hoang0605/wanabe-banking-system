using Accounts.Models;
using Accounts.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Features.AuditAccountBalance;

internal class AuditAccountBalanceService : IAuditAccountBalanceService
{
    private readonly IAccountManagementContext _context;

    public AuditAccountBalanceService(IAccountManagementContext context)
    {
        _context = context;
    }

    public async Task<AuditAccountBalanceResultDto?> ExecuteAsync(string accountNumber)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null) return null;

        var entries = await _context.AccountLedgerEntries
            .Where(e => e.AccountNumber == accountNumber)
            .ToListAsync();

        // Tính toán động lại cán cân từ lịch sử log
        double totalCredits = entries.Where(e => e.Type == EntryType.Credit).Sum(e => e.Amount);
        double totalDebits = entries.Where(e => e.Type == EntryType.Debit).Sum(e => e.Amount);
        double calculatedBalance = totalCredits - totalDebits;

        // Kiểm tra tính toàn vẹn dữ liệu giữa Snapshot và tổng dòng lịch sử
        bool isIntegrityValid = (Math.Abs(account.Balance - calculatedBalance) < 0.001);

        return new AuditAccountBalanceResultDto(
            accountNumber,
            account.Balance,
            calculatedBalance,
            entries.Count,
            isIntegrityValid ? "SECURE (100% Match)" : "CRITICAL WARNING: UNAUTHORIZED TAMPERING DETECTED!"
        );
    }
}