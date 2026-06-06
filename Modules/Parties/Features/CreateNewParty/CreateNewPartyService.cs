using Microsoft.EntityFrameworkCore;
using Parties.Models;
using Parties.Models.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parties.Features.CreateNewParty
{
    internal class CreateNewPartyService : ICreateNewPartyService
    {
        private readonly IPartyManagementContext _context;

        public CreateNewPartyService(IPartyManagementContext context)
        {
            _context = context;
        }
        public async Task<Boolean> CreateNewParty(PartyRequestDto request)
        {
            if (String.IsNullOrWhiteSpace(request.Email)) return false;
            var email = await _context.Parties.AnyAsync(p => p.Email == request.Email.Trim());
            if (!email) return false;

            var newParty = new Party
            {
                PartyId = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                DateCreated = DateTime.Now,
                KycStatus = Kycstatus.Pending
            };
            _context.Parties.Add(newParty);
            return true;
        }
    }
}
