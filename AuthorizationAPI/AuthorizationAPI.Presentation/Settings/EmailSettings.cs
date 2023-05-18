namespace AuthorizationAPI.Presentation.Settings
{
    public class EmailSettings
    {
        public string SenderName { get; set; }

        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
    }
}
