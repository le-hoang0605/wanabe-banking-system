namespace Transactions.DTOs
{
    public record TransferSessionDto(Guid PaymentId, Guid DebtorAccountId, Guid CreditorAccountId, double Amount);
}
