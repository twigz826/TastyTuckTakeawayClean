using TastyTuckTakeaway.Core.Helpers;

namespace TastyTuckTakeaway.Core.Tests.Helpers
{
    public class PasscodeManagerTests
    {
        [Fact]
        public void PasscodeManager_VerifyPasscode_ReturnsTrueWhenAValidPasscodeIsEntered()
        {
            var passcodeManager = new PasscodeManager();
            var userInput = "001001";
            var oneTimePasscode = "001001";

            var result = passcodeManager.VerifyPasscode(userInput, oneTimePasscode);

            result.Should().BeTrue();
        }

        [Fact]
        public void PasscodeManager_VerifyPasscode_ReturnsFalseWhenAnInvalidPasscodeIsEntered()
        {
            var passcodeManager = new PasscodeManager();
            var userInput = "0";
            var oneTimePasscode = "001001";

            var result = passcodeManager.VerifyPasscode(userInput, oneTimePasscode);

            result.Should().BeFalse();
        }
    }
}