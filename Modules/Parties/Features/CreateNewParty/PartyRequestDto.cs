using System;
using System.Collections.Generic;
using System.Text;

namespace Parties.Features.CreateNewParty
{
    public record PartyRequestDto
    (
        string FullName,
        string Email
    );
}
