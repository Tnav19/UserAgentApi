using Microsoft.EntityFrameworkCore;
using UserAgentApi.Models;

namespace UserAgentApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Agent> Agents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure one-to-many relationship
            modelBuilder.Entity<Agent>()
                .HasMany(a => a.Users)
                .WithOne(u => u.Agent)
                .HasForeignKey(u => u.AgentId)
                .OnDelete(DeleteBehavior.SetNull);  // Optional: set null if agent is deleted

            base.OnModelCreating(modelBuilder);
        }
    }
}
