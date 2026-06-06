using System.ComponentModel.DataAnnotations;

namespace Transactions.DTOs
{
    public record CreateTransferRequestDto(
        [property: Required(ErrorMessage = "Debtor account ID is required.")]
        Guid DebtorAccountId,
        [property: Required(ErrorMessage = "Creditor account ID is required.")]
        Guid CreditorAccountId,
        [property: Range(0.01, double.MaxValue, ErrorMessage = "Amount must be strictly greater than zero.")]
        double Amount);
}
