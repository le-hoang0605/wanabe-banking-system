using System.ComponentModel.DataAnnotations;

namespace Transactions.DTOs
{
    public record TransferRequestWithKeyDto(
        [property: Required(ErrorMessage = "Debtor account ID is required.")]
        Guid DebtorAccountId,
        [property: Required(ErrorMessage = "Creditor account ID is required.")]
        Guid CreditorAccountId,
        [property: Range(0.01, double.MaxValue, ErrorMessage = "Amount must be strictly greater than zero.")]
        double Amount,
        [property: Required(ErrorMessage = "Idempotency key is required.")]
        [property: StringLength(256, ErrorMessage = "Idempotency key cannot exceed 256 characters.")]
        string IdempotencyKey);
}
