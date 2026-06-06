namespace Accounts.Features.PostLedgerEntry;

public interface IAccountService
{
    // Lệnh trừ tiền
    Task<AccountOpResult> DebitAsync(string accountNumber, double amount, Guid transactionId);
    
    // Lệnh cộng tiền
    Task<AccountOpResult> CreditAsync(string accountNumber, double amount, Guid transactionId);
}