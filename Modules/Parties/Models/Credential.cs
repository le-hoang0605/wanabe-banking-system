namespace Parties.Models;

internal class Credential
{
    public Party Party { get; set; }
    public Guid PartyId { get; set; } // Credential is a weak entity, this (PartyId) is a foreign key.
    public string PasswordHashed { get; set; } //add salt to hashed password plz
    public DateTime UpdatedAt { get; set; }
}