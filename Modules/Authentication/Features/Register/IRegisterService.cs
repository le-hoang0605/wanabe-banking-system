using System;
using System.Collections.Generic;
using System.Text;

namespace Authentications.Features.Register
{
    public interface IRegisterService
    {
        public Task<Boolean> Register(RegisterRequestDto request);
    }
}
