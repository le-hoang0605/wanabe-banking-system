using Authentications.Models.Context;
using Microsoft.EntityFrameworkCore;


namespace Authentications.Features.Authenticate
{
    internal class AuthenticateService : IAuthenticateService
    {
        private readonly AuthenticationManagementContext _context;
        public AuthenticateService(AuthenticationManagementContext context)
        {
            _context = context;
        }
        public async Task<Boolean> Authenticate(AuthenticateRequestDto request)
        {
            if (String.IsNullOrWhiteSpace(request.PartyId.ToString()) || String.IsNullOrWhiteSpace(request.HashPassword))
                return false;

            var credential = await _context.Credentials.FirstOrDefaultAsync(c => c.PartyId == request.PartyId);
            if (credential == null) return false;

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.HashPassword, credential.PasswordHashed);
            if (!isPasswordValid) return false;

            return true;

        }
        
    }
}
