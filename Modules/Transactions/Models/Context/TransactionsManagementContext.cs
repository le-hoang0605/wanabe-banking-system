using Microsoft.EntityFrameworkCore;
using Transactions.Models;

namespace Transactions.Models.Context;

internal class TransactionsManagementContext : DbContext, ITransactionsManagementContext
{
    public TransactionsManagementContext(DbContextOptions<TransactionsManagementContext> options) : base(options) { }

    public DbSet<PaymentOrder> PaymentOrders { get; set; }
    public DbSet<LedgerEntry> LedgerEntries { get; set; }

    public new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PaymentOrder>().HasKey(p => p.PaymentId);
        modelBuilder.Entity<PaymentOrder>().Property(p => p.IdempotencyKey).IsRequired().HasMaxLength(255);
        modelBuilder.Entity<PaymentOrder>().Property(p => p.DebtorAccountId).IsRequired();
        modelBuilder.Entity<PaymentOrder>().Property(p => p.CreditorAccountId).IsRequired();
        modelBuilder.Entity<PaymentOrder>().Property(p => p.Amount).IsRequired();
        modelBuilder.Entity<PaymentOrder>().Property(p => p.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        modelBuilder.Entity<PaymentOrder>().Property(p => p.CreatedAt).IsRequired();
        
        modelBuilder.Entity<PaymentOrder>().HasIndex(p => p.IdempotencyKey).IsUnique();
        
        
        modelBuilder.Entity<LedgerEntry>().HasKey(e => e.EntryId);
        modelBuilder.Entity<LedgerEntry>().Property(e => e.AccountId).IsRequired();
        modelBuilder.Entity<LedgerEntry>().Property(e => e.TransactionType).HasConversion<string>().HasMaxLength(10).IsRequired();
        modelBuilder.Entity<LedgerEntry>().Property(e => e.Amount).IsRequired();
        modelBuilder.Entity<LedgerEntry>().Property(e => e.CreatedAt).IsRequired();


        modelBuilder.Entity<LedgerEntry>()
            .HasOne(e => e.paymentOrder)
            .WithMany(p => p.LedgerEntries)
            .HasForeignKey(e => e.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}