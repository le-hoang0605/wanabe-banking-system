namespace Accounts.Models;

public class Account
{
    public Guid AccountId { get; set; }
    public Guid PartyId { get; set; }
    public string AccountNumber { get; set; }
    public double Balance { get; set; } = 0;
    public string Currency { get; set; } = "VND";
    public Status Status { get; set; } //Active, Suspended
    public Role Role { get; set; } //User, Manager
}

public enum Status
{
    Active, Suspended
}
public enum Role
{
    User, Manager
}