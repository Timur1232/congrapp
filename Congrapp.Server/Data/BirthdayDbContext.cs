using Congrapp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Data;

public class BirthdayDbContext(DbContextOptions<BirthdayDbContext> options) : DbContext(options)
{
    public DbSet<BirthdayInfo> BirthdayInfos { get; set; }
    public DbSet<BirthdayNote> BirthdayNotes { get; set; }
}