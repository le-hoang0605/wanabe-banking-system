using Accounts.Features.ChangeAccountStatus;
using Accounts.Features.CreateAccount;
using Accounts.Features.GetAccountByNumber;
using Accounts.Features.GetAccountsByPartyId;
using Accounts.Features.PostLedgerEntry;
using Accounts.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DBConnection");
     
        services.AddDbContext<AccountManagementContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IAccountManagementContext>(provider =>
            provider.GetRequiredService<AccountManagementContext>());


        services.AddScoped<Features.PostLedgerEntry.IAccountService, Features.PostLedgerEntry.AccountService>();

        services.AddScoped<IGetAccountByNumberService, GetAccountByNumberService>();
        services.AddScoped<IGetAccountsByPartyIdService, GetAccountsByPartyIdService>();
        services.AddScoped<ICreateAccountService,
            CreateAccountService>();
        services.AddScoped<IChangeAccountStatusService, ChangeAccountStatusService>();
        services.AddScoped<IAccountService, AccountService>();

        return services;
    }
}