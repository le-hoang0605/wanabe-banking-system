using System;
using System.Collections.Generic;
using System.Text;

namespace Authentications.Features.Authenticate
{
    public record AuthenticateRequestDto
    (
        Guid PartyId,
        string HashPassword
    );
}
