using System;
using System.Collections.Generic;
using System.Text;

namespace Parties.Features.FindPartyViaEmail
{
    public interface IFindPartyViaEmailService
    {
        public Task<PartyResponseDto> FindPartyViaEmail(string email);
    }
}
