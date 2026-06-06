using Microsoft.EntityFrameworkCore;

namespace Transactions.Models.Context
{
    internal class TransactionManagementContext : DbContext, ITransactionManagementContext
    {
        public TransactionManagementContext(DbContextOptions<TransactionManagementContext> options) : base(options) { }
        public DbSet<LedgerEntry> LedgerEntries { get; set; }
        public DbSet<PaymentOrder> PaymentOrders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("transaction-domain"); //create separate schema 

            //ledgerEntry
            modelBuilder.Entity<LedgerEntry>(entity =>
            {
                entity.HasKey(l => l.EntryId);

                entity.Property(l => l.TransactionType).HasConversion<string>().HasMaxLength(10).IsRequired();
                entity.Property(l => l.AccountId).IsRequired();
                entity.Property(l => l.Amount).IsRequired();
                entity.Property(l => l.CreatedAt).IsRequired();

                entity.HasOne(l => l.PaymentOrder).WithMany(p => p.LedgerEntries).HasForeignKey(l => l.PaymentId).OnDelete(DeleteBehavior.Cascade); // delete an order also deletes its related ledger
            });

            //paymentOrder
            modelBuilder.Entity<PaymentOrder>(entity =>
            {
                entity.HasKey(p => p.PaymentId);

                entity.Property(p => p.IdempotencyKey).IsRequired().HasMaxLength(256);
                entity.HasIndex(p => p.IdempotencyKey).IsUnique();

                entity.Property(p => p.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
                entity.Property(p => p.DebtorAccountId).IsRequired();
                entity.Property(p => p.CreditorAccountId).IsRequired();
                entity.Property(p => p.Amount).IsRequired();

            });
        }
    }
}
