using AuthorizationAPI.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationAPI.Application.Interfaces
{
    public interface IAuthorizationContext
    {
        DbSet<User> Users { get; set; }
        Task<int> SaveChangesAsync();
    }
}
