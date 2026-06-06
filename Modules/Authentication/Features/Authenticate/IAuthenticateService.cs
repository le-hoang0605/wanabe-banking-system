using System;
using System.Collections.Generic;
using System.Text;

namespace Authentications.Features.Authenticate
{
    public interface IAuthenticateService
    {
        public Task<Boolean> Authenticate(AuthenticateRequestDto request);
    }
}
