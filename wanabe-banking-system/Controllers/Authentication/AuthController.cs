using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using wanabe_banking_system.UseCases;

namespace wanabe_banking_system.Controllers.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LoginOrchestrator _loginOrchestrator;

        public AuthController(LoginOrchestrator loginOrchestrator)
        {
            _loginOrchestrator = loginOrchestrator;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _loginOrchestrator.LoginAsync(request.Email, request.Password);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Error = "Unexpected error. The server burned down or sth" });
            }
        }
    }
}
