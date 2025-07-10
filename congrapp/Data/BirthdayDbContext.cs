using congrapp.Models;
using Microsoft.EntityFrameworkCore;

namespace congrapp.Database;

public class BirthdayDbContext : DbContext
{
    public DbSet<BirthdayInfo> BirthdayInfos { get; set; }
    public DbSet<BirthdayNote> BirthdayNotes { get; set; }
    
    public BirthdayDbContext(DbContextOptions<BirthdayDbContext> options)
        : base(options)
    {
    }
}