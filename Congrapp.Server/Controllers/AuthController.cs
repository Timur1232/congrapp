using Congrapp.Server.Data;
using Congrapp.Server.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly BirthdayDbContext _birthdayDbContext;
    private readonly IPasswordHasher _passwordHasher; 
    private readonly IJwtTokenProvider _jwtTokenProvider;
    
    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Email, string Password, string PasswordConfirmation);

    public AuthController(IConfiguration config, BirthdayDbContext birthdayDbContext, IPasswordHasher passwordHasher, IJwtTokenProvider jwtTokenProvider)
    {
        _birthdayDbContext = birthdayDbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenProvider = jwtTokenProvider;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _birthdayDbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
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
        if (request.Password != request.PasswordConfirmation)
        {
            return Unauthorized("Passwords do not match.");
        }
        
        var existingUser = await _birthdayDbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
        if (existingUser != null)
        {
            return Unauthorized("User already exists.");
        }
        
        string passwordHash = _passwordHasher.Hash(request.Password);
        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash
        };
        
        _birthdayDbContext.Users.Add(user);
        await _birthdayDbContext.SaveChangesAsync();
        
        return CreatedAtAction(nameof(Login), new LoginRequest(request.Email, request.Password), user);
    }
    // public void Varify() {}
}