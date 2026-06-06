using System;
using System.Collections.Generic;
using System.Text;

namespace Parties.Features.CreateNewParty
{
    public interface ICreateNewPartyService
    {
        public Task<Boolean> CreateNewParty(PartyRequestDto data);
     }
}
