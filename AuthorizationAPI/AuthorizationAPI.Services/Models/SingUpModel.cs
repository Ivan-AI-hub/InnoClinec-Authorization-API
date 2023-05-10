using AuthorizationAPI.Domain;


namespace AuthorizationAPI.Services.Models
{
    public record SingUpModel(string Email,
                          string Password,
                          string RePassword,
                          Role Role);
}
