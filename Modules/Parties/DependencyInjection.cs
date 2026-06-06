using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parties
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPartiesModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DBConnection");

            services.AddDbContext<Models.Context.PartyManagementContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddScoped<Models.Context.IPartyManagementContext>(provider =>
             provider.GetRequiredService<Models.Context.PartyManagementContext>());

            services.AddScoped<Parties.Features.CreateNewParty.ICreateNewPartyService, 
                Parties.Features.CreateNewParty.CreateNewPartyService>();
            services.AddScoped<Parties.Features.FindPartyViaEmail.IFindPartyViaEmailService, 
                Parties.Features.FindPartyViaEmail.FindPartyViaEmailService>();

            return services;
        }
    }
}
