using FakeItEasy;
using TastyTuckTakeaway.Core.Services;

namespace TastyTuckTakeaway.Core.Tests.Services
{
    public class PaymentProcessingServiceTests
    {
        [Fact]
        public async Task PaymentProcessingService_ProcessPayment_ReturnsTrueWhenPaymentIsSuccessful()
        {
            var paymentProcessorService = A.Fake<IPaymentProcessingService>();
            var paymentAmount = 100;
            var validPaymentInfo = "1234567890123456#0124#123";
            A.CallTo(() => paymentProcessorService.ProcessPayment(paymentAmount, validPaymentInfo))
                .ReturnsNextFromSequence(true);

            var paymentResult = await paymentProcessorService.ProcessPayment(paymentAmount, validPaymentInfo);
            paymentResult.Should().Be(true);
        }

        [Fact]
        public async Task PaymentProcessingService_ProcessPayment_ReturnsFalseWhenPaymentIsUnsuccessful()
        {
            var paymentProcessorService = A.Fake<IPaymentProcessingService>();
            var paymentAmount = 100;
            var validPaymentInfo = "1234567890123456#0124#123";
            A.CallTo(() => paymentProcessorService.ProcessPayment(paymentAmount, validPaymentInfo))
                .ReturnsNextFromSequence(false);

            var paymentResult = await paymentProcessorService.ProcessPayment(paymentAmount, validPaymentInfo);
            paymentResult.Should().Be(false);
        }
    }
}