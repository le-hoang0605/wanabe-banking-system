using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Transactions.Models.Context;
using Transactions.Features.ExecuteTransfer;

namespace Transactions;

public static class DependencyInjection
{
    public static IServiceCollection AddTransactionsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Đăng ký lớp Context quản lý mới của bạn
        services.AddDbContext<TransactionsManagementContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ITransactionsManagementContext>(provider =>
            provider.GetRequiredService<TransactionsManagementContext>());

        // Đăng ký Feature Service
        services.AddScoped<IExecuteTransferService, ExecuteTransferService>();

        return services;
    }
}