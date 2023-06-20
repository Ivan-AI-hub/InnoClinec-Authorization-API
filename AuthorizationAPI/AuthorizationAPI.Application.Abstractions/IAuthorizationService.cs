using AuthorizationAPI.Application.Abstractions.Models;

namespace AuthorizationAPI.Application.Abstractions
{
    public interface IAuthorizationService
    {
        Task ChangeRoleAsync(string email, RoleDTO role, CancellationToken cancellationToken = default);

        /// <summary>
        /// Confirms email for the user with a specific id
        /// </summary>
        Task ConfirmEmailAsync(ConfirmEmailModel model, CancellationToken cancellationToken = default);

        /// <summary>
        /// Registers the patient in system
        /// </summary>
        Task<UserDTO> SingUpAsync(SingUpModel model, RoleDTO role, CancellationToken cancellationToken = default);
        public string GetAccessToken(string email, string password);
    }
}