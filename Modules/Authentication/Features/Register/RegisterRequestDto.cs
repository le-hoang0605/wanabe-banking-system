using System;
using System.Collections.Generic;
using System.Text;

namespace Authentications.Features.Register
{
    public record RegisterRequestDto
    (
        Guid PartyId,
        string HashPassword
    );
}
