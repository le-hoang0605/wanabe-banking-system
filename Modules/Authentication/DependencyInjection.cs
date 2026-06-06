using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Authentications
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthenticationsModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DBConnection");

            services.AddDbContext<Models.Context.AuthenticationManagementContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<Authentications.Features.Register.IRegisterService,
                Authentications.Features.Register.RegisterService>();
            services.AddScoped<Authentications.Features.Authenticate.IAuthenticateService,
                Authentications.Features.Authenticate.AuthenticateService>();

            return services;
        }
    }
}
