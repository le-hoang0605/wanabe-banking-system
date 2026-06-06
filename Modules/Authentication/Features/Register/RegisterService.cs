using Authentications.Models;
using Authentications.Models.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Authentications.Features.Register
{
    internal class RegisterService : IRegisterService
    {
        private readonly IAuthenticationManagementContext _context;
        public RegisterService(IAuthenticationManagementContext context)
        {
            _context = context;
        }
        public async Task<Boolean> Register(RegisterRequestDto request)
        {
            if (String.IsNullOrWhiteSpace(request.PartyId.ToString()) || String.IsNullOrWhiteSpace(request.HashPassword))
                return false;
            var credential = await _context.Credentials.AnyAsync(c => c.PartyId == request.PartyId);
            if (credential) return false;

            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            string passwordHashedWithSalt = BCrypt.Net.BCrypt.HashPassword(request.HashPassword, salt);

            var newCredential = new Credential
            {
                PartyId = request.PartyId,
                PasswordHashed = passwordHashedWithSalt,
                UpdatedAt = DateTime.Now
            };
            _context.Credentials.Add(newCredential);
            return true;
        }
    }
}
