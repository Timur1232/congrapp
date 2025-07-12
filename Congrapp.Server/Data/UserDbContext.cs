using Congrapp.Server.Users;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
}