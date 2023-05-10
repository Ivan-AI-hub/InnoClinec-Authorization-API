namespace AuthorizationAPI.Services.Abstractions.Models
{
    public record SingUpModel(string Email,
                          string Password,
                          string RePassword);
}
