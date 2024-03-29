﻿using AuthorizationAPI.Domain.Repositories;

namespace AuthorizationAPI.Persistence.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly AuthorizationContext _context;

        private Lazy<IUserRepository> _userRepository;
        public RepositoryManager(AuthorizationContext context)
        {
            _context = context;
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(context));
        }

        public IUserRepository UserRepository => _userRepository.Value;
    }
}
