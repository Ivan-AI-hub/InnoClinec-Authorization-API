namespace AuthorizationAPI.Domain.Exceptions
{
    public class EmailAreNotUniqueException : BadRequestException
    {
        public EmailAreNotUniqueException()
            : base("User with the same Email exist in the database.")
        {
        }
    }
}
