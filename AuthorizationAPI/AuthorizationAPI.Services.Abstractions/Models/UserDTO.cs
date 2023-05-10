namespace AuthorizationAPI.Services.Abstractions.Models
{
    public class UserDTO
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public RoleDTO Role { get; private set; }
    }
}
