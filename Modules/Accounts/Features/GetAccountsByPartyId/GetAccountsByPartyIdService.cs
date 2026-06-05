using Accounts.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Features.GetAccountsByPartyId;

internal class GetAccountsByPartyIdService : IGetAccountsByPartyIdService
{
    private readonly IAccountManagementContext _context;

    public GetAccountsByPartyIdService(IAccountManagementContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AccountPartyDetailDto>> ExecuteAsync(Guid partyId)
    {
        var accounts = await _context.Accounts
            .Where(a => a.PartyId == partyId)
            .Select(a => new AccountPartyDetailDto(
                a.AccountId,
                a.PartyId,
                a.AccountNumber,
                a.Balance,
                a.Currency,
                a.Status,
                a.Role,
                a.CreatedAt
            ))
            .ToListAsync();

        return accounts;
    }
}