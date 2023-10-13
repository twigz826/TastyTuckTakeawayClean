using FakeItEasy;
using TastyTuckTakeaway.Core.Enums;
using TastyTuckTakeaway.Core.Models;
using TastyTuckTakeaway.Core.Models.EmailResultTokens;
using TastyTuckTakeaway.Core.Services;

namespace TastyTuckTakeaway.Core.Tests.Services
{
    public class EmailServiceTests
    {
        [Fact]
        public async Task SendEmailAsync_WhenVerification_ShouldReturnVerificationEmailResultToken()
        {
            // Arrange
            var emailService = A.Fake<IEmailService>();
            var emailAddress = "test@example.com";
            var testOtp = "123456";

            // Setup behavior for SendEmailAsync when emailType is Verification
            A.CallTo(() => emailService.SendEmailAsync(emailAddress, EmailTypes.Verification, 0, A<IEnumerable<OrderItem>>.Ignored))
                .ReturnsNextFromSequence(new VerificationEmailResultToken(true, emailAddress, testOtp));

            // Act
            var result = await emailService.SendEmailAsync(emailAddress, EmailTypes.Verification);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<VerificationEmailResultToken>();
            result.Should().BeAssignableTo<EmailResultTokenBase>();
            result.As<VerificationEmailResultToken>().Success.Should().BeTrue();
            result.As<VerificationEmailResultToken>().EmailAddress.Should().Be(emailAddress);
            result.As<VerificationEmailResultToken>().OTP.Should().Be(testOtp);
        }

        [Fact]
        public async Task SendEmailAsync_WhenConfirmation_ShouldReturnConfirmationEmailResultToken()
        {
            // Arrange
            var emailService = A.Fake<IEmailService>();
            var emailAddress = "test@example.com";
            var orderNumber = 123456;
            var orderItems = new List<OrderItem> { OrderItem.Create(new MenuItem(1, "Spring Rolls", 5, "starters"), 1) };

            // Setup behavior for SendEmailAsync when emailType is Verification
            A.CallTo(() => emailService.SendEmailAsync(emailAddress, EmailTypes.OrderConfirmation, orderNumber, orderItems))
                .ReturnsNextFromSequence(new OrderConfirmationEmailResultToken(true, emailAddress, orderNumber));

            // Act
            var result = await emailService.SendEmailAsync(emailAddress, EmailTypes.OrderConfirmation, orderNumber, orderItems);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OrderConfirmationEmailResultToken>();
            result.Should().BeAssignableTo<EmailResultTokenBase>();
            result.As<OrderConfirmationEmailResultToken>().Success.Should().BeTrue();
            result.As<OrderConfirmationEmailResultToken>().EmailAddress.Should().Be(emailAddress);
            result.As<OrderConfirmationEmailResultToken>().OrderNumber.Should().Be(orderNumber);
        }
    }
}