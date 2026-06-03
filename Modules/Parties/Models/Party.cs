namespace Parties.Models;

internal class Party
{
    public Credential Credential { get; set; }
    public Guid PartyId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public Kycstatus KycStatus { get; set; }
}

public enum Kycstatus
{
    Pending, Approved, Rejected, Suspended
}