using Accounts.Features.CreateAccount;
using Accounts.Models.Context;
using Authentications.Features.Register;
using Authentications.Models.Context;
using Parties.Features.CreateNewParty;
using Parties.Features.FindPartyViaEmail;
using Parties.Models.Context;

namespace wanabe_banking_system.UseCases.RegisterOrchestrator
{
    // to register, we need to:
    // find if the party is alr available,
    // create new party, create new account, create new credential
    public class RegisterOrchestrator
    {
        private readonly IFindPartyViaEmailService _findPartyService;
        private readonly ICreateNewPartyService _createPartyService;
        private readonly ICreateAccountService _accountService;
        private readonly IRegisterService _registerService;

        public RegisterOrchestrator(IFindPartyViaEmailService findPartyService,
                                    ICreateNewPartyService createAccountService,
                                    ICreateAccountService accountService,
                                    IRegisterService registerService)
        {
            _findPartyService = findPartyService;
            _createPartyService = createAccountService;
            _accountService = accountService;
            _registerService = registerService;
        }

        public async Task<Boolean> RegisterAsync(RegisterOchestratorRequestDto request)
        {
            // find a matching party
            var partyResult = await _findPartyService.FindPartyViaEmail(request.Email);
            if (partyResult != null) throw new UnauthorizedAccessException("User alr exists!");
            
            // create new party
            var newPartyResult =
                await _createPartyService.CreateNewParty(new PartyRequestDto(request.FullName, request.Email));
            if (newPartyResult == null) throw new UnauthorizedAccessException("Validate your data");
            
            //create new (default) account
            await _accountService.ExecuteAsync(new CreateAccountRequestDto(
                        newPartyResult.PartyId, 0, "VND", false));

            // create new credentials
            bool a = await _registerService.Register(new RegisterRequestDto(newPartyResult.PartyId, request.Password));
            if (!a) throw new UnauthorizedAccessException("sth wrong happened");
            
            return true;
        }
        

    }
}
