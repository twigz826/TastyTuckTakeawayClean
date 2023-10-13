namespace TastyTuckTakeaway.Core.Models.EmailResultTokens
{
    public class OrderConfirmationEmailResultToken : EmailResultTokenBase
    {
        public OrderConfirmationEmailResultToken(bool success, string emailAddress, int orderNumber)
            : base(success, emailAddress)
        {
            OrderNumber = orderNumber;
        }

        public int OrderNumber { get; set; }
    }
}