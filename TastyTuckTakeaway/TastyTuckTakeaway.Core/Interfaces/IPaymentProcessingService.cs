using TastyTuckTakeaway.Core.Models;

namespace TastyTuckTakeaway.Core.Interfaces
{
    public interface IPaymentProcessingService
    {
        Task<bool> ProcessPayment(double amount, string paymentInfo, AppUser user, int orderId);
    }
}