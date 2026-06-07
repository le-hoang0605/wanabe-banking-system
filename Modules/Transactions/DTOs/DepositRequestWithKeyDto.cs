namespace Transactions.DTOs
{
    public record DepositRequestWithKeyDto(
         Guid CreditorAccountId,
         double Amount,
         string IdempotencyKey
     );
    // i use this one for running in system so i already give the clean data to this (i dont need to validate it again here)
}
