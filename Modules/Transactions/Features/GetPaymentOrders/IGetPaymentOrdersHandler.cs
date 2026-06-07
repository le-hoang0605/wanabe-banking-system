using Transactions.DTOs;

namespace Transactions.Features.GetPaymentOrders
{
    public interface IGetPaymentOrdersHandler
    {
        Task<IEnumerable<PaymentOrderDto>> HandleAsync(Guid accountId, int pageNumber, int pageSize);
    }
}
