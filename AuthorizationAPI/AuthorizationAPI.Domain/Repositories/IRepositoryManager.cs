namespace AuthorizationAPI.Domain.Repositories
{
    public interface IRepositoryManager
    {
        IUserRepository UserRepository { get; }
    }
}
