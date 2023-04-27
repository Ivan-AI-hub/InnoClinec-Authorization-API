using AuthorizationAPI.Application.Interfaces;
using AuthorizationAPI.DAL.EntityConfiguration;
using AuthorizationAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationAPI.DAL
{
    public class AuthorizationContext : DbContext, IAuthorizationContext
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
