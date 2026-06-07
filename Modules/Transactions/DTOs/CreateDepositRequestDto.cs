using System.ComponentModel.DataAnnotations;

namespace Transactions.DTOs
{
    public record CreateDepositRequestDto(
         [Required(ErrorMessage = "Creditor account ID is required!")]
        Guid CreditorAccountId,
         [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be strictly greater than zero!")]
        double Amount
     );
}
