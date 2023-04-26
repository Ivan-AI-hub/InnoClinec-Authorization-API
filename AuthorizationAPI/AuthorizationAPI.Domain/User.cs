namespace AuthorizationAPI.Domain
{
    public class User
    {
        public string Email { get; private set; }
        public Role Role { get; private set; }
        public string PasswordHach { get; private set; }

        private User() { }
        public User(string email, Role role, string passwordHach)
        {
            Email = email;
            Role = role;
            PasswordHach = passwordHach;
        }
    }
}
