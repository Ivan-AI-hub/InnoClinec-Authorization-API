using AuthorizationAPI.Persistence.EntityConfiguration;
using AuthorizationAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationAPI.Persistence
{
    public class AuthorizationContext : DbContext
    {
        public AuthorizationContext(DbContextOptions<AuthorizationContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfigurator());
        }
    }
}
