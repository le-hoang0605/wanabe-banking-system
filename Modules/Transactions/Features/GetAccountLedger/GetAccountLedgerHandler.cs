using Microsoft.EntityFrameworkCore;
using Transactions.DTOs;
using Transactions.Models.Context;

namespace Transactions.Features.GetAccountLedger
{
    internal class GetAccountLedgerHandler : IGetAccountLedgerHandler
    {
        private readonly TransactionManagementContext _context;

        public GetAccountLedgerHandler(TransactionManagementContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<LedgerEntryDto>> HandleAsync(Guid accountId, int pageNumber, int pageSize)
        {
            return await _context.LedgerEntries
                .AsNoTracking()
                .Where(l => l.AccountId == accountId)
                .OrderByDescending(l => l.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)  //paginatuon starts here
                .Take(pageSize)
                .Select(l => new LedgerEntryDto(
                    l.EntryId,
                    l.PaymentId,
                    l.TransactionType.ToString(),
                    l.Amount,
                    l.CreatedAt))
                .ToListAsync();
        }
    }
}
