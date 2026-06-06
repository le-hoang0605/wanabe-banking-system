namespace Parties.Models;

public class Party
{
    public Guid PartyId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public Kycstatus KycStatus { get; set; }
    public DateTime DateCreated { get; set; }
}

public enum Kycstatus
{
    Pending, Approved, Rejected, Suspended
}