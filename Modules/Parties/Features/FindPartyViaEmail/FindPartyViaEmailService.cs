using Microsoft.EntityFrameworkCore;
using Parties.Models.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parties.Features.FindPartyViaEmail
{
    internal class FindPartyViaEmailService : IFindPartyViaEmailService
    {
        private readonly IPartyManagementContext _context;
        public FindPartyViaEmailService(IPartyManagementContext context) { _context = context; }
        public async Task<PartyResponseDto?> FindPartyViaEmail(string email)
        {
            if (String.IsNullOrWhiteSpace(email)) return null;
            var party = await _context.Parties.FirstOrDefaultAsync(p => p.Email == email.Trim());
            if (party == null) return null;
            return new PartyResponseDto(party.PartyId, party.FullName, party.KycStatus.ToString());
        }
    }
}
