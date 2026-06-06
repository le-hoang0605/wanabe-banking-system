using Accounts.Models;
using Accounts.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Features.PostLedgerEntry;

internal class AccountService : IAccountService
{
    private readonly IAccountManagementContext _context;

    public AccountService(IAccountManagementContext context)
    {
        _context = context;
    }

    // HÀM XỬ LÝ TRỪ TIỀN (DEBIT)
    public async Task<AccountOpResult> DebitAsync(string accountNumber, double amount, Guid transactionId)
    {
        // 1. Tìm tài khoản gửi
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null) 
            return new AccountOpResult(false, "Tài khoản gửi không tồn tại.");

        // 2. Kiểm tra trạng thái tài khoản
        if (account.Status == Status.Suspended) 
            return new AccountOpResult(false, "Tài khoản gửi đang bị khóa.");

        // 3. Kiểm tra số dư tài khoản gửi
        if (account.Balance < amount) 
            return new AccountOpResult(false, "Số dư tài khoản không đủ để thực hiện giao dịch.");

        // 4. Hợp lệ thì thực hiện trừ tiền trực tiếp trên thực thể
        account.Balance -= amount;
        account.UpdatedAt = DateTime.UtcNow;

        // 5. Lưu cập nhật số dư vào DB
        await _context.SaveChangesAsync();

        return new AccountOpResult(true, string.Empty);
    }

    // HÀM XỬ LÝ CỘNG TIỀN (CREDIT)
    public async Task<AccountOpResult> CreditAsync(string accountNumber, double amount, Guid transactionId)
    {
        // 1. Tìm tài khoản nhận
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null) 
            return new AccountOpResult(false, "Tài khoản nhận không tồn tại.");

        // 2. Kiểm tra trạng thái tài khoản nhận
        if (account.Status == Status.Suspended) 
            return new AccountOpResult(false, "Tài khoản nhận đang bị khóa, không thể nhận tiền.");

        // 3. Hợp lệ thì thực hiện cộng tiền trực tiếp
        account.Balance += amount;
        account.UpdatedAt = DateTime.UtcNow;

        // 4. Lưu cập nhật số dư vào DB
        await _context.SaveChangesAsync();

        return new AccountOpResult(true, string.Empty);
    }
}