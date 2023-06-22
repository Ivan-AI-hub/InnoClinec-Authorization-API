namespace AuthorizationAPI.Application.Abstractions.Models
{
    public class AccessToken
    {
        public string Token { get; set; }

        public AccessToken(string token)
        {
            Token = token;
        }
    }
}
