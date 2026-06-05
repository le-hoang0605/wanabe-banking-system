using Accounts.Models;

namespace Accounts.Features.GetAccountsByPartyId;

public record AccountPartyDetailDto(
    Guid AccountId,
    Guid PartyId,
    string AccountNumber,
    double Balance,
    string Currency,
    Status Status,
    Role Role,
    DateTime CreatedAt
);