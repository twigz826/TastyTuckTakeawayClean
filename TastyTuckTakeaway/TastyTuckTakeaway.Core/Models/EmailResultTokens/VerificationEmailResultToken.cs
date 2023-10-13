namespace TastyTuckTakeaway.Core.Models.EmailResultTokens
{
    public class VerificationEmailResultToken : EmailResultTokenBase
    {
        public VerificationEmailResultToken(bool success, string emailAddress, string otp)
            : base(success, emailAddress)
        {
            OTP = otp;
        }

        public string OTP { get; set; } = string.Empty;
    }
}