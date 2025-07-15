using Congrapp.Server.Data;
using Congrapp.Server.Models;
using Congrapp.Server.Services;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    BirthdayDbContext birthdayDbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenProvider jwtTokenProvider,
    EmailVerificationService emailVerificationService)
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
            return NotFound(new {Error = "User not found."});
        }

        if (!passwordHasher.Varify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new {Error = "Invalid password."});
        }

        var token = jwtTokenProvider.GenerateJwtToken(user);
        return Ok(new {Token = token});
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request.Password != request.PasswordConfirmation)
        {
            return Unauthorized(new {Error = "Passwords do not match."});
        }
        
        var existingUser = await birthdayDbContext.Users.SingleOrDefaultAsync(x => x.Email == request.Email);
        if (existingUser != null)
        {
            return Unauthorized(new {Error = "User already exists."});
        }
        
        var passwordHash = passwordHasher.Hash(request.Password);
        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash
        };
        
        birthdayDbContext.Users.Add(user);
        await birthdayDbContext.SaveChangesAsync();

        var emailVerification = await emailVerificationService.SendVerificationEmailAsync(user);
        birthdayDbContext.EmailVerifications.Add(emailVerification);
        await birthdayDbContext.SaveChangesAsync();

        var token = jwtTokenProvider.GenerateJwtToken(user);
        return Ok(new {Token = token});
    }

    [HttpGet("verify")]
    public async Task<IActionResult> Verify([FromQuery] string token)
    {
        var inputToken = Guid.Parse(token);
        
        var emailVerification = await birthdayDbContext.EmailVerifications
            .SingleOrDefaultAsync(x => x.Id == inputToken);
        if (emailVerification == null)
        {
            return BadRequest(new {Error = "Invalid token."});
        }
        
        var user = await birthdayDbContext.Users.FirstOrDefaultAsync(x => x.Id == emailVerification.UserId);
        birthdayDbContext.EmailVerifications.Remove(emailVerification);
        if (user == null)
        {
            return NotFound(new {Error = "User not found."});
        }
        
        user.EmailVerified = true;
        birthdayDbContext.Users.Update(user);
        await birthdayDbContext.SaveChangesAsync();
        
        var userDto = new User.UserDto(user.Email, user.EmailVerified);
        return Ok(userDto);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound(new {Error = "User not found."});
        }
        
        var userDto = new User.UserDto(user.Email, user.EmailVerified);
        return Ok(userDto);
    }
}