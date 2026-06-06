namespace Accounts.Features.GetAccountsByPartyId;

public interface IGetAccountsByPartyIdService
{
    Task<IEnumerable<AccountPartyDetailDto>> ExecuteAsync(Guid partyId);
}