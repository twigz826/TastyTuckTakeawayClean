using TastyTuckTakeaway.Core.Models;
using TastyTuckTakeaway.Core.Models.EmailResultTokens;

namespace TastyTuckTakeaway.Core.Interfaces
{
    public interface IEmailService
    {
        Task<EmailResultTokenBase?> SendEmailAsync(string emailAddress, string emailType, int orderNumber = 0, IEnumerable<OrderItem>? orderItems = null);

        bool IsValidEmail(string emailAddress);
    }
}