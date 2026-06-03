namespace Accounts.Models;

internal class Account
{
    public Guid AccountId { get; set; }
    public Guid PartyId { get; set; }
    public string AccountNumber { get; set; }
    public double Balance { get; set; }
    public string Currency { get; set; }
    public Status Status { get; set; }
}

public enum Status
{
    Active, Suspended
}