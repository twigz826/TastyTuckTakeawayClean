namespace TastyTuckTakeaway.Core.Helpers
{
    public interface IPasscodeManager
    {
        bool VerifyPasscode(string oneTimePasscode, string userInput);

        string GenerateOTP();
    }
    public class PasscodeManager : IPasscodeManager
    {
        public bool VerifyPasscode(string oneTimePasscode, string userInput)
        {
            if (int.TryParse(oneTimePasscode, out var otp) && int.TryParse(userInput, out var userOtp))
            {
                return otp == userOtp;
            }
            return false;
        }

        public string GenerateOTP()
        {
            var random = new Random();
            var otp = random.Next(100000, 999999);
            return otp.ToString();
        }

    }
}