using System.ComponentModel.DataAnnotations;

namespace Transactions.DTOs
{
    public record TransferRequestWithKeyDto(
        [Required(ErrorMessage = "Debtor account ID is required.")]
        Guid DebtorAccountId,
        [Required(ErrorMessage = "Creditor account ID is required.")]
        Guid CreditorAccountId,
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be strictly greater than zero.")]
        double Amount,
        [Required(ErrorMessage = "Idempotency key is required.")]
        [StringLength(256, ErrorMessage = "Idempotency key cannot exceed 256 characters.")]
        string IdempotencyKey);
}
