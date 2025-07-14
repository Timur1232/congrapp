using System.Security.Claims;
using Congrapp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Data;

public class BirthdayDbContext(DbContextOptions<BirthdayDbContext> options) : DbContext(options)
{
    public DbSet<BirthdayInfo> BirthdayInfos { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<EmailVerification> EmailVerifications { get; set; }
    public DbSet<NotificationRecord> NotificationRecords { get; set; }

    public async Task<User?> GetUserByClaims(ClaimsPrincipal userClaims)
    {
        var idClaim = userClaims.FindFirst("userId");
        if (idClaim == null || string.IsNullOrWhiteSpace(idClaim.Value))
        {
            return null;
        }
        
        int userId = int.Parse(idClaim.Value);
        var user = await Users.FindAsync(userId);
        return user;
    }
}