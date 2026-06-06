using System;
using System.Collections.Generic;
using System.Text;

namespace Parties.Features.FindPartyViaEmail
{
    public record PartyResponseDto
    (
        Guid PartyId,
        string FullName,
        string KycStatus
    );
}
