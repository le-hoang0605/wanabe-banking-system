namespace Transactions.Models;

public class PaymentOrder
{
    public ICollection<LedgerEntry> LedgerEntries { get; set; } = new List<LedgerEntry>();
    public Guid PaymentId { get; set; }
    public string IdempotencyKey { get; set; }
    public Guid DebtorAccountId { get; set; } //references AccountId (Account). Don't actually reference this as navigation property plz.
    public Guid CreditorAccountId { get; set; }
    public double Amount { get; set; }     //PHAN CO THE gây lỗi sai số nhất nhớ đổi sau khi hoàn thiện
    public Status Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
public enum Status
{
    Initiated, Processing, Executed, Failed
}