using Accounts.Features.GetAccountsByPartyId;
using Authentications.Features.Authenticate;
using Authentications.Features.GenerateToken;
using Parties.Features.FindPartyViaEmail;

namespace wanabe_banking_system.UseCases
{
    public class LoginOrchestrator
    {
        private readonly IAuthenticateService _authService;
        private readonly IFindPartyViaEmailService _partyService;
        private readonly IGenerateToken _tokenGenerator;
        private readonly IGetAccountsByPartyIdService _accountService;

        public LoginOrchestrator(
        IAuthenticateService authService,
        IFindPartyViaEmailService partyService,
        IGenerateToken tokenGenerator,
        IGetAccountsByPartyIdService accontService)
        {
            _authService = authService;
            _partyService = partyService;
            _tokenGenerator = tokenGenerator;
            _accountService = accontService;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            // find a matching party
            var partyResult = await _partyService.FindPartyViaEmail(email);

            if (partyResult == null) throw new UnauthorizedAccessException("Invalid credentials");

            // find a matching credential
            var authResult = await _authService.Authenticate(new AuthenticateRequestDto(
                (Guid)partyResult.PartyId,password));

            if (!authResult) throw new UnauthorizedAccessException("Invalid credentials");

            // find matching account(s)
            // ...
            var accountResult = await _accountService.ExecuteAsync((Guid)partyResult.PartyId);
            List<string> roles = new List<string>();
            foreach (var account in accountResult)
            {
                roles.Add(account.Role.ToString());
            }

            // generate token
            string token = _tokenGenerator.GenerateJwtToken(
                new TokenRequestDto(
                    (Guid)partyResult.PartyId, 
                    partyResult.FullName, 
                    email,roles , 
                    partyResult.KycStatus.ToString())
            );

            return token;            
        }

    }
}
