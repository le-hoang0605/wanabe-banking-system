using Accounts.Models;

namespace Accounts.Features.GetAccountByNumber;

public record GetAccountByNumberResultDto(
    Guid AccountId,
    Guid PartyId,
    string AccountNumber,
    double Balance,
    string Currency,
    Status Status,
    Role Role,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);