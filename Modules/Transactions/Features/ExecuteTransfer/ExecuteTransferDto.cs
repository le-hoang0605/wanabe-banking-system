using System.ComponentModel.DataAnnotations;

namespace Transactions.Features.ExecuteTransfer;

public record ExecuteTransferRequestDto(
    [Required] string IdempotencyKey,
    [Required] string DebtorAccountNumber,   
    [Required] string CreditorAccountNumber, 
    [Required] [Range(0.01, double.MaxValue)] double Amount
);

public record ExecuteTransferResponseDto(
    Guid PaymentId,
    string Status,
    string Message,
    DateTime CreatedAt
);