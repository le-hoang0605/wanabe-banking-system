using System.ComponentModel.DataAnnotations;

namespace Accounts.Features.CreateAccount;

public record CreateAccountRequestDto(
    [Required] Guid PartyId,
    [Required] [Range(0, double.MaxValue)] double InitialBalance,
    [Required] [StringLength(3)] string Currency,
    bool IsManager
);

public record CreateAccountResponseDto(
    Guid AccountId,
    string AccountNumber,
    double Balance,
    string Currency,
    string Status
);