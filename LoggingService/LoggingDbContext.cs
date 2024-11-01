using Microsoft.EntityFrameworkCore;
using LoggingService.Models;

public class LoggingDbContext : DbContext
{
    public DbSet<LogEntry> LogEntries { get; set; }

    public LoggingDbContext(DbContextOptions<LoggingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogEntry>().HasKey(l => l.Id);
    }
}
