using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Transactions.Features.GetAccountLedger;
using Transactions.Features.GetPaymentOrders;
using Transactions.Features.TransferMoney;
using Transactions.Models.Context;

namespace Transactions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddTransactionsModule(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<TransactionManagementContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddScoped<ITransactionManagementContext>(provider =>
                provider.GetRequiredService<TransactionManagementContext>());


            services.AddScoped<ITransferMoneyHandler, TransferMoneyHandler>();
            services.AddScoped<IGetPaymentOrdersHandler, GetPaymentOrdersHandler>();
            services.AddScoped<IGetAccountLedgerHandler, GetAccountLedgerHandler>();
            return services;
        }
    }
}

