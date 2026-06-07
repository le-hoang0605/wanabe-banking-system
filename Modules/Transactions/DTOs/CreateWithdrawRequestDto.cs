using System.ComponentModel.DataAnnotations;

namespace Transactions.DTOs
{
    public record CreateWithdrawRequestDto(
         [Required(ErrorMessage = "Debtor account ID is required!")]
        Guid DebtorAccountId,
         [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be strictly greater than zero!")]
        double Amount
     );
}
