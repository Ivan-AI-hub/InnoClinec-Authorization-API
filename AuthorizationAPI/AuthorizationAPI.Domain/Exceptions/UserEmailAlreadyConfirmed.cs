
namespace AuthorizationAPI.Domain.Exceptions
{
    public class UserEmailAlreadyConfirmed : BadRequestException
    {
        public UserEmailAlreadyConfirmed(string email) :
            base($"User's email address {email} has already been confirmed")
        {
        }
    }
}
