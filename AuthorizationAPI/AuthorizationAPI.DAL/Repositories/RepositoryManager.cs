using AuthorizationAPI.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationAPI.DAL.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private AuthorizationContext _context;

        private Lazy<IUserRepository> _userRepository;
        public RepositoryManager(AuthorizationContext context)
        {
            _context = context;
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(context));
        }

        public IUserRepository UserRepository => _userRepository.Value;

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
