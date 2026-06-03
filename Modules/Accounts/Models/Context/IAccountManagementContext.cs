using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Models.Context
{
    public interface IAccountManagementContext
    {
        DbSet<Account> Accounts { get; set; }
    }
}
