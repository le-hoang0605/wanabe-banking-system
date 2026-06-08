using System;
using System.Collections.Generic;
using System.Text;
using Accounts.Models.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.Accounts
{ 
    public class TestDbContextFactory
    {
        internal static AccountManagementContext Create()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var optionsBuilder = new DbContextOptionsBuilder<AccountManagementContext>();
            optionsBuilder.UseSqlite(connection);

            var context = new AccountManagementContext(optionsBuilder.Options);
            context.Database.EnsureCreated(); 

            return context;
        }
    }
}
