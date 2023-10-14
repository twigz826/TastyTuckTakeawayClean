namespace TastyTuckTakeaway.Core.Configuration
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;

        public int Port { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}