using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parties.Models.Context
{
    public interface IPartyManagementContext
    {
        DbSet<Party> Parties { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
