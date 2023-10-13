namespace TastyTuckTakeaway.Core.Models.EmailResultTokens
{
    public class EmailResultTokenBase
    {
        public EmailResultTokenBase(bool success, string emailAddress)
        {
            Success = success;
            EmailAddress = emailAddress;
        }

        public bool Success { get; set; }

        public string EmailAddress { get; set; } = string.Empty;
    }
}