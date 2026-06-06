using Authentications.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authentications.Models.Context
{
    public interface IAuthenticationManagementContext
    {
        DbSet<Credential> Credentials { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
