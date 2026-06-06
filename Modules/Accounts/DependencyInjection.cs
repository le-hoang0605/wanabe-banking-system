using Accounts.Features.AuditAccountBalance;
using Accounts.Features.ChangeAccountStatus;
using Accounts.Features.CreateAccount;
using Accounts.Features.GetAccountByNumber;
using Accounts.Features.GetAccountsByPartyId;
using Accounts.Features.PostLedgerEntry;
using Accounts.Models;
using Accounts.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accounts;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        //Tạm thời chuyển sang dùng InMemoryDatabase để test liên kết mô-đun
        services.AddDbContext<AccountManagementContext>(options =>
            options.UseSqlServer(connectionString));
        
        services.AddScoped<IAccountManagementContext>(provider =>
            provider.GetRequiredService<AccountManagementContext>());
        // You can also register internal services/repositories here if you have them
        // services.AddScoped<IAccountService, AccountService>();
        // Đăng ký Feature Service chính thức
        services.AddScoped<Features.PostLedgerEntry.IPostLedgerEntryService, Features.PostLedgerEntry.PostLedgerEntryService>();
        
        services.AddScoped<IGetAccountByNumberService, GetAccountByNumberService>();
        services.AddScoped<IGetAccountsByPartyIdService, GetAccountsByPartyIdService>();
        services.AddScoped<ICreateAccountService, CreateAccountService>();
        services.AddScoped<IPostLedgerEntryService, PostLedgerEntryService>();
        services.AddScoped<IAuditAccountBalanceService, AuditAccountBalanceService>();
        services.AddScoped<IChangeAccountStatusService, ChangeAccountStatusService>();
        
        return services;
    }
}