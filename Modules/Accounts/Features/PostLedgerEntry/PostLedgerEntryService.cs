using Accounts.Models;
using Accounts.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Features.PostLedgerEntry;

internal class PostLedgerEntryService : IPostLedgerEntryService
{
    private readonly IAccountManagementContext _context;

    public PostLedgerEntryService(IAccountManagementContext context)
    {
        _context = context;
    }
    
    public async Task<(bool IsSuccess, string ErrorMessage, AccountPostingResponseDto? Data)> ExecuteAsync(AccountPostingRequestDto dto)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == dto.AccountNumber);

        if (account == null)
            return (false, "Bank account number does not exist.", null);

        if (account.Status == Status.Suspended)
            return (false, "Account is suspended. Posting rejected.", null);

        // Business Lgoic: Đảm bảo đủ số dư khi thực hiện giao dịch ghi Nợ (Debit - Rút/Chuyển đi)
        if (dto.Type == EntryType.Debit && account.Balance < dto.Amount)
        {
            return (false, "Transaction rejected. Insufficient account balance.", null);
        }

        // Chèn một hàng bất biến (Immutable log) vào Sổ cái chi tiết giao dịch nội bộ
        var ledgerEntry = new AccountLedgerEntry
        {
            EntryId = Guid.NewGuid(),
            AccountNumber = dto.AccountNumber,
            Type = dto.Type,
            Amount = dto.Amount,
            TransactionId = dto.TransactionId, // Đồng bộ liên kết chéo với Module Transactions
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow
        };
        _context.AccountLedgerEntries.Add(ledgerEntry);

        // Đồng bộ hóa bản vá bộ nhớ đệm Read-Cache Snapshot (Trường số dư Balance)
        if (dto.Type == EntryType.Credit)
            account.Balance += dto.Amount;
        else
            account.Balance -= dto.Amount;

        account.UpdatedAt = DateTime.UtcNow;

        //Cam kết toàn bộ đơn vị công việc một cách toàn vẹn vào DB
        await _context.SaveChangesAsync();

        var responseData = new AccountPostingResponseDto(
            "Ledger entry posted successfully.",
            account.Balance,
            ledgerEntry.EntryId
        );

        return (true, string.Empty, responseData);
    }
}