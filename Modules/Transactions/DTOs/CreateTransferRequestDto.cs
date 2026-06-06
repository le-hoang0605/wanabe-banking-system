namespace Transactions.DTOs
{
    public record CreateTransferRequestDto(Guid DebtorAccountId, Guid CreditorAccountId, double Amount);
}
