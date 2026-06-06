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
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null) 
            return new AccountOpResult(false, "Account not exist.");

      
        if (account.Status == Status.Suspended) 
            return new AccountOpResult(false, "Suspeneded Account.");

        if (account.Balance < amount) 
            return new AccountOpResult(false, "Not sufficient account.");

        
        account.Balance -= amount;
        account.UpdatedAt = DateTime.UtcNow;


        await _context.SaveChangesAsync();

        return new AccountOpResult(true, string.Empty);
    }

    // HÀM XỬ LÝ CỘNG TIỀN (CREDIT)
    public async Task<AccountOpResult> CreditAsync(string accountNumber, double amount, Guid transactionId)
    {
      
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null) 
            return new AccountOpResult(false, "Account not exist.");

        if (account.Status == Status.Suspended) 
            return new AccountOpResult(false, "Suspeneded Account.");

       
        account.Balance += amount;
        account.UpdatedAt = DateTime.UtcNow;

   
        await _context.SaveChangesAsync();

        return new AccountOpResult(true, string.Empty);
    }
}