namespace Transactions.DTOs
{
    public record WithdrawRequestWithKeyDto(
        Guid DebtorAccountId,
        double Amount,
        string IdempotencyKey
    );
}
