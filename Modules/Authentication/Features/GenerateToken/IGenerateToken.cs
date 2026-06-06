using System;
using System.Collections.Generic;
using System.Text;

namespace Authentications.Features.GenerateToken
{
    public interface IGenerateToken
    {
        public string GenerateJwtToken(TokenRequestDto request);
    }
}
