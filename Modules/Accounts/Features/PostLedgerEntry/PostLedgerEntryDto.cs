using Accounts.Models;
using System.ComponentModel.DataAnnotations;

namespace Accounts.Features.PostLedgerEntry;

public record AccountPostingRequestDto(
    [Required] string AccountNumber,
    [Required] EntryType Type,
    [Required] [Range(0.01, double.MaxValue)] double Amount,
    [Required] Guid TransactionId,
    [Required] [StringLength(255)] string Description
);

public record AccountPostingResponseDto(
    string Message,
    double CurrentBalance,
    Guid EntryId
);