using AuthorizationAPI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthorizationAPI.DAL.EntityConfiguration
{
    internal class UserConfigurator : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(u => u.Email).IsUnique();

            builder.Property(u => u.Email).IsRequired();
            builder.Property(u => u.Role).IsRequired();
            builder.Property(u => u.PasswordHach).IsRequired();
        }
    }
}
