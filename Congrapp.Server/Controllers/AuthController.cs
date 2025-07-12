using Congrapp.Server.Data;
using Congrapp.Server.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserDbContext _userDbContext;
    private readonly PasswordHasher _passwordHasher = new PasswordHasher();
    private readonly JwtTokenProvider _jwtTokenProvider;
    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Email, string Password);

    public AuthController(IConfiguration config, UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
        _jwtTokenProvider = new JwtTokenProvider(config);
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userDbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
        if (user == null)
        {
            return Unauthorized();
        }

        if (!_passwordHasher.Varify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid password");
        }

        return Ok(_jwtTokenProvider.GenerateJwtToken(user));
    }
}