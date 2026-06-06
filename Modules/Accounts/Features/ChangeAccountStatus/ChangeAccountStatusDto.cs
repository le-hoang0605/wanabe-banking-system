using Accounts.Models;
using System.ComponentModel.DataAnnotations;

namespace Accounts.Features.ChangeAccountStatus;

public record ChangeAccountStatusRequestDto(
    [Required] Status NewStatus
);

public record ChangeAccountStatusResponseDto(
    string Message,
    string AccountNumber,
    Status CurrentStatus,
    DateTime UpdatedAt
);