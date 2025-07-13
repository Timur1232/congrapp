using Congrapp.Server.Data;
using Congrapp.Server.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    BirthdayDbContext birthdayDbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenProvider jwtTokenProvider)
    : ControllerBase
{
    public record LoginRequest(string Email, string Password);
    public record RegisterRequest(string Email, string Password, string PasswordConfirmation);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await birthdayDbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
        if (user == null)
        {
            return Unauthorized("User not found.");
        }

        if (!passwordHasher.Varify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid password.");
        }

        string token = jwtTokenProvider.GenerateJwtToken(user);
        return Ok(token);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request.Password != request.PasswordConfirmation)
        {
            return Unauthorized("Passwords do not match.");
        }
        
        var existingUser = await birthdayDbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
        if (existingUser != null)
        {
            return Unauthorized("User already exists.");
        }
        
        string passwordHash = passwordHasher.Hash(request.Password);
        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash
        };
        
        birthdayDbContext.Users.Add(user);
        await birthdayDbContext.SaveChangesAsync();
        
        return CreatedAtAction(nameof(Login), new LoginRequest(request.Email, request.Password), user);
    }
    // public void Varify() {}
}