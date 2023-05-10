namespace AuthorizationAPI.Domain.Exceptions
{
    public class UserAuthenticationException : NotFoundException
    {
        public UserAuthenticationException()
            : base("the user email or password data does not match with the existing data")
        {
        }
    }
}
