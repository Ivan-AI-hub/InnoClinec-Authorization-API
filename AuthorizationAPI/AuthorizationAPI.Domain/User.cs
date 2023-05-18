namespace AuthorizationAPI.Domain
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public bool IsEmailConfirmed { get; set; }
        public Role Role { get; private set; }
        public string PasswordHach { get; private set; }

        private User() { }
        public User(string email, Role role, string passwordHach)
        {
            Id = Guid.NewGuid();
            Email = email;
            Role = role;
            PasswordHach = passwordHach;
        }
    }
}
