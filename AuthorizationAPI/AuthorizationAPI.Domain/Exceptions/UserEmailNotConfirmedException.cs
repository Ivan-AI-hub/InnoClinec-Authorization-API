namespace AuthorizationAPI.Domain.Exceptions
{
    public class UserEmailNotConfirmedException : BadRequestException
    {
        public UserEmailNotConfirmedException(string email) 
            : base($"The email address {email} has not been confirmed")
        {
        }
    }
}
