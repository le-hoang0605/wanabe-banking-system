using Accounts.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Features.GetAccountByNumber;

internal class GetAccountByNumberService : IGetAccountByNumberService
{
    private readonly IAccountManagementContext _context;

    public GetAccountByNumberService(IAccountManagementContext context)
    {
        _context = context;
    }

    public async Task<GetAccountByNumberResultDto?> ExecuteAsync(string accountNumber)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

        if (account == null) return null;

        return new GetAccountByNumberResultDto(
            account.AccountId,
            account.PartyId,
            account.AccountNumber,
            account.Balance,
            account.Currency,
            account.Status,
            account.Role,
            account.CreatedAt,
            account.UpdatedAt
        );
    }
}