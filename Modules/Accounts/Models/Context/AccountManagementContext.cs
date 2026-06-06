using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Accounts.Models.Context
{
    internal class AccountManagementContext : DbContext,IAccountManagementContext
    {
        public AccountManagementContext(DbContextOptions<AccountManagementContext> options) : base(options){ }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountLedgerEntry> AccountLedgerEntries { get; set; }

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
            modelBuilder.Entity<Account>().HasKey(a => a.AccountId);
            modelBuilder.Entity<Account>().Property(a => a.AccountNumber).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Account>().Property(a => a.Role).HasConversion<string>().HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Account>().Property(a => a.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Account>().Property(a => a.Currency).HasMaxLength(3).IsRequired();
            modelBuilder.Entity<Account>().Property(a => a.PartyId).IsRequired();
            modelBuilder.Entity<Account>().Property(a => a.Balance).IsRequired();
            modelBuilder.Entity<Account>().Property(a => a.CreatedAt).IsRequired();
            modelBuilder.Entity<Account>().Property(a => a.UpdatedAt);


            modelBuilder.Entity<AccountLedgerEntry>().HasKey(e => e.EntryId);
            modelBuilder.Entity<AccountLedgerEntry>().Property(e => e.AccountNumber).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<AccountLedgerEntry>().Property(e => e.Type).HasConversion<string>().HasMaxLength(10).IsRequired();
            modelBuilder.Entity<AccountLedgerEntry>().Property(e => e.Amount).IsRequired();
            modelBuilder.Entity<AccountLedgerEntry>().Property(e => e.TransactionId).IsRequired();
            modelBuilder.Entity<AccountLedgerEntry>().Property(e => e.Description).HasMaxLength(255).IsRequired();
            modelBuilder.Entity<AccountLedgerEntry>().Property(e => e.CreatedAt).IsRequired();

            // Database Index for high-performance ledger querying
            modelBuilder.Entity<AccountLedgerEntry>().HasIndex(e => e.AccountNumber);
        }
    }
}
