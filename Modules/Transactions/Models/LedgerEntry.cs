namespace Transactions.Models;

public class LedgerEntry
{
    public PaymentOrder paymentOrder { get; set; }
    public Guid EntryId { get; set; }
    public Guid PaymentId { get; set; } 
    public Guid AccountId { get; set; } //references AccountId (Account). Dont actually reference this as navigation properties.
    public TransactionType TransactionType { get; set; }
    public double Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum TransactionType
{
    Credit, Debit
}