namespace AuthorizationAPI.Application.Abstractions.Models
{
    public record SingUpModel(string Email,
                          string Password,
                          string RePassword);
}
