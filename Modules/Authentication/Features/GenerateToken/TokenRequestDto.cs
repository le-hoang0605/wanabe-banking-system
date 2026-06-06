using System;
using System.Collections.Generic;
using System.Text;

namespace Authentications.Features.GenerateToken
{
    public record TokenRequestDto
    (Guid PartyId, string Name, string Email ,List<string> Roles, string KycStatus);
}
