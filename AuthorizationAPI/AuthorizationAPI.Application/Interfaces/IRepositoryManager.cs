namespace AuthorizationAPI.Application.Interfaces
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }
        public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
