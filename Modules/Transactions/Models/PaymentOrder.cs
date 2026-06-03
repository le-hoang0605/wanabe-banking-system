using System.Data;

namespace Transactions.Models;

internal class PaymentOrder
{
    public ICollection<LedgerEntry> LedgerEntries { get; set; } = new List<LedgerEntry>();
    public Guid PaymentId { get; set; }
    public string IdempotencyKey { get; set; }
    public Guid DebtorAccountId { get; set; } //references AccountId (Account). Don't actually reference this as navigation property plz.
    public Guid CreditorAccountId { get; set; }
    public double Amount { get; set;}
    public Status Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
public enum Status
{
    Initiated, Processing, Executed, Failed
}