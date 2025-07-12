using Congrapp.Server.Data;
using Congrapp.Server.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserDbContext _userDbContext;
    private readonly PasswordHasher _passwordHasher; 
    private readonly JwtTokenProvider _jwtTokenProvider;
    
    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Email, string Password);

    public AuthController(IConfiguration config, UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
        _jwtTokenProvider = new JwtTokenProvider(config);
        _passwordHasher = new PasswordHasher();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userDbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
        if (user == null)
        {
            return Unauthorized("User not found.");
        }

        if (!_passwordHasher.Varify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid password.");
        }

        string token = _jwtTokenProvider.GenerateJwtToken(user);
        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingUser = await _userDbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
        if (existingUser != null)
        {
            return Unauthorized("User already exists.");
        }
        
        string passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
        };
        
        _userDbContext.Users.Add(user);
        await _userDbContext.SaveChangesAsync();
        
        return CreatedAtAction(nameof(Login), new LoginRequest(request.Email, request.Password), user);
    }
    // public void Varify() {}
}