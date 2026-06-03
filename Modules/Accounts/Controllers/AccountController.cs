using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Accounts.Models.Context;
namespace Accounts.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountManagementContext _context;
    public AccountController(IAccountManagementContext context)
    {
        _context = context;
    }
    [HttpGet]
    public IActionResult Test()
    {
        return Ok("safe and sound");
    }

}