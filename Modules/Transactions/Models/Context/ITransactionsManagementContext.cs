using Microsoft.EntityFrameworkCore;
using Transactions.Models;

namespace Transactions.Models.Context;

public interface ITransactionsManagementContext
{
    DbSet<PaymentOrder> PaymentOrders { get; set; }
    DbSet<LedgerEntry> LedgerEntries { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}