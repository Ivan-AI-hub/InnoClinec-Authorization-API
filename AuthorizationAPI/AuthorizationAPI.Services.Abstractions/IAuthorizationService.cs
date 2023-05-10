using AuthorizationAPI.Services.Abstractions.Models;

namespace AuthorizationAPI.Services.Abstractions
{
    public interface IAuthorizationService
    {
        Task ConfirmEmailAsync(ConfirmEmailModel model, CancellationToken cancellationToken = default);
        Task<string> GetAccessTokenAsync(GetAccessTokenModel model, CancellationToken cancellationToken = default);
        Task<UserDTO> SingUpAsync(SingUpModel model, RoleDTO role, CancellationToken cancellationToken = default);
    }
}