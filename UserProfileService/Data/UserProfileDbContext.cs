using Microsoft.EntityFrameworkCore;
using UserProfileService.Models;

namespace UserProfileService.Data
{
    public class UserProfileDbContext : DbContext
    {
        public UserProfileDbContext(DbContextOptions<UserProfileDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
