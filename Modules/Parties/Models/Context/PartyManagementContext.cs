using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parties.Models.Context
{
    internal class PartyManagementContext : DbContext, IPartyManagementContext
    {
        
        public PartyManagementContext(DbContextOptions<PartyManagementContext> options) : base(options){ }
        public DbSet<Party> Parties { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //party-domain schema declaration
            modelBuilder.HasDefaultSchema("party-domain");

            // 1. Cấu hình cho Entity Party
            modelBuilder.Entity<Party>(entity =>
            {
                entity.HasKey(p => p.PartyId); 

                entity.Property(p => p.FullName)
                      .IsRequired()
                      .HasMaxLength(150); 

                entity.Property(p => p.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                // unique index
                entity.HasIndex(p => p.Email)
                      .IsUnique();

                //Enum representation from int to string
                entity.Property(p => p.KycStatus)
                      .HasConversion<string>()
                      .HasMaxLength(30)
                      .IsRequired();

                entity.Property(p => p.DateCreated).IsRequired();
            });
        }
    }
}
