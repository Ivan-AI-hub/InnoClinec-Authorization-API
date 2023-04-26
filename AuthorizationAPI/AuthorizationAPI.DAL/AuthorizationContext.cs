using AuthorizationAPI.Application.Interfaces;
using AuthorizationAPI.DAL.EntityConfiguration;
using AuthorizationAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationAPI.DAL
{
    public class AuthorizationContext : DbContext, IAuthorizationContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfigurator());
        }
        public Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync();
        }
    }
}
