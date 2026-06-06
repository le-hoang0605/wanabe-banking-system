using Microsoft.EntityFrameworkCore;
using Authentications.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authentications.Models.Context
{
    internal class AuthenticationManagementContext : DbContext
    {
        public DbSet<Credential> Credentials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("authentication_domain");

            modelBuilder.Entity<Credential>(entity =>
            {
                // weak entity
                entity.HasKey(c => c.PartyId);

                entity.Property(c => c.PasswordHashed)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(c => c.UpdatedAt)
                      .IsRequired();
            });
        }
    }
}
