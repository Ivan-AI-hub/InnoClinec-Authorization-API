using AuthorizationAPI.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AuthorizationAPI.DAL.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private AuthorizationContext _context;
        private IServiceProvider _serviceProvider;

        private IUserRepository _userRepository;
        public RepositoryManager(AuthorizationContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                return _userRepository;
            }
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
