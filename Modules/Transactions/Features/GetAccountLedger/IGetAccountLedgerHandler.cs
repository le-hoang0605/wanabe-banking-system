using Transactions.DTOs;

namespace Transactions.Features.GetAccountLedger
{
    public interface IGetAccountLedgerHandler
    {
        // adding pagination
        Task<IEnumerable<LedgerEntryDto>> HandleAsync(Guid accountId, int pageNumber, int pageSize);

    }
}
