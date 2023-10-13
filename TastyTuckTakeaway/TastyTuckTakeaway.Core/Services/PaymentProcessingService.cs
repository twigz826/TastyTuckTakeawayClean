namespace TastyTuckTakeaway.Core.Services
{
    public interface IPaymentProcessingService
    {
        Task<bool> ProcessPayment(double amount, string paymentInfo);
    }

    public class PaymentProcessingService : IPaymentProcessingService
    {
        public async Task<bool> ProcessPayment(double amount, string paymentInfo)
        {
            var paymentProviderApiCall = await TakePaymentViaExtenalProvider(amount, paymentInfo);
            return paymentProviderApiCall;
        }

        private async Task<bool> TakePaymentViaExtenalProvider(double amount, string paymentInfo)
        {
            await Task.Delay(2500);
            return await Task.FromResult(true);
        }
    }
}