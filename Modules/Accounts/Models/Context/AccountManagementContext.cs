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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().HasKey(a => a.AccountId);
            modelBuilder.Entity<Account>().Property(a => a.AccountNumber).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Account>().Property(a => a.Role).HasConversion<string>().HasMaxLength(50).IsRequired();
            modelBuilder.Entity<Account>().Property(a => a.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
            modelBuilder.Entity<Account>().Property(a => a.Currency).HasMaxLength(3).IsRequired();
            modelBuilder.Entity<Account>().Property(a => a.PartyId).IsRequired();
            modelBuilder.Entity<Account>().Property(a => a.Balance).IsRequired();
        }
    }
}
