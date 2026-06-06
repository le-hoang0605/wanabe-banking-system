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
        public async Task<CreatePartyResponseDto> CreateNewParty(PartyRequestDto request)
        {
            if (String.IsNullOrWhiteSpace(request.Email)) return null;
            var email = await _context.Parties.AnyAsync(p => p.Email == request.Email.Trim());
            if (!email) return null;

            var newParty = new Party
            {
                PartyId = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                DateCreated = DateTime.Now,
                KycStatus = Kycstatus.Pending
            };
            _context.Parties.Add(newParty);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Internal server error");
                
            }
            return new CreatePartyResponseDto(newParty.PartyId);
        }
    }
}
