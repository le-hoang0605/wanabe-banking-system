namespace Transactions.DTOs
{
    public record TransferRequestWithKeyDto(Guid DebtorAccountId, Guid CreditorAccountId, double Amount, string IdempotencyKey
    );
}
