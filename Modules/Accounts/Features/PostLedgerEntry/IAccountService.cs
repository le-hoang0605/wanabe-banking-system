namespace Accounts.Features.PostLedgerEntry;

public interface IAccountService
{
    // Lệnh trừ tiền
    Task<AccountOpResult> DebitAsync(Guid accountId, double amount, Guid transactionId);
    
    // Lệnh cộng tiền
    Task<AccountOpResult> CreditAsync(Guid accountId, double amount, Guid transactionId);
}