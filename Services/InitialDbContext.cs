using Microsoft.EntityFrameworkCore;

namespace LeaderboardApp.Services
{
    public class InitialDbContext : DbContext
    {
        public InitialDbContext(DbContextOptions<InitialDbContext> options)
            : base(options)
        {
        }

        // No DbSet<> needed here because we're only using this context for executing raw SQL, no entities
    }
}
