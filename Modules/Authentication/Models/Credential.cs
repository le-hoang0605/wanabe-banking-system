namespace Authentications.Models;

public class Credential
{
    public Guid PartyId { get; set; }
    public string PasswordHashed { get; set; } //add salt to hashed password plz
    public DateTime UpdatedAt { get; set; }
}