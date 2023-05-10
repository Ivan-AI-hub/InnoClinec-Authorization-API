using AuthorizationAPI.DAL.Abstracts;
using AuthorizationAPI.Domain;
using AuthorizationAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationAPI.DAL.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(AuthorizationContext context) : base(context)
        {
        }

        public override IQueryable<User> GetItems(bool trackChanges)
        {
            var users = Context.Users;

            return trackChanges ? users : users.AsNoTracking();
        }
    }
}
