namespace AuthorizationAPI.Domain.Interfaces
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }
        /// <summary>
        /// Saves all changes made in repositories.
        /// </summary>
        public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
