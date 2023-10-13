using System.Globalization;

namespace TastyTuckTakeaway.Core.Helpers
{
    public interface IPaymentManager
    {
        bool IsValidPaymentInfo(string? paymentInfo);
    }

    public class PaymentManager : IPaymentManager
    {
        public bool IsValidPaymentInfo(string? paymentInfo)
        {
            if (String.IsNullOrEmpty(paymentInfo))
            {
                return false;
            }

            string[] paymentDetails = paymentInfo.Split('#');
            if (ExistThreePartsInUserInput(paymentDetails) && IsValidCardNumber(paymentDetails[0]) && IsValidExpiryDate(paymentDetails[1]) && IsValidCVC(paymentDetails[2]))
            {
                return true;
            }
            return false;
        }

        private static bool ExistThreePartsInUserInput(string[] paymentDetails)
        {
            return paymentDetails.Length == 3;
        }

        private static bool IsValidCVC(string cvc)
        {
            return cvc.Length == 3 && int.TryParse(cvc, out _);
        }

        private static bool IsValidExpiryDate(string date)
        {
            if (!DateTime.TryParseExact(date, "MMyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expiryDate))
            {
                return false;
            }
            return DateTime.Now <= expiryDate;
        }

        private static bool IsValidCardNumber(string cardNumber)
        {
            return cardNumber.Length == 16 && long.TryParse(cardNumber, out _);
        }
    }
}