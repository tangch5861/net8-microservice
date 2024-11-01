using Microsoft.EntityFrameworkCore;
using AuthService.Models;

public class AuthDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
    }
}
