using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using wanabe_banking_system.UseCases;
using wanabe_banking_system.UseCases.RegisterOrchestrator;

namespace wanabe_banking_system.Controllers.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LoginOrchestrator _loginOrchestrator;
        private readonly RegisterOrchestrator _registerOrchestrator;

        public AuthController(LoginOrchestrator loginOrchestrator, RegisterOrchestrator registerOrchestrator)
        {
            _loginOrchestrator = loginOrchestrator;
            _registerOrchestrator = registerOrchestrator;
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
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterOchestratorRequestDto request)
        {
            try
            {
                bool a = await _registerOrchestrator.RegisterAsync(request);
                return Ok("Registered Successfully");
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
