using Microsoft.EntityFrameworkCore;
using Transactions.DTOs;
using Transactions.Models.Context;

namespace Transactions.Features.GetPaymentOrders
{
    internal class GetPaymentOrdersHandler : IGetPaymentOrdersHandler
    {
        private readonly TransactionManagementContext _context;
        public GetPaymentOrdersHandler(TransactionManagementContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<PaymentOrderDto>> HandleAsync(Guid accountId, int pageNumber, int pageSize)
        {
            return await _context.PaymentOrders
                .AsNoTracking()
                .Where(p => p.DebtorAccountId == accountId || p.CreditorAccountId == accountId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PaymentOrderDto(
                    p.PaymentId,
                    p.DebtorAccountId,
                    p.CreditorAccountId,
                    p.Amount,
                    p.Status.ToString(),
                    p.CreatedAt))
                .ToListAsync();
        }
    }
}
