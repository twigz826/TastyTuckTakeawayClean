using TastyTuckTakeaway.Core.Interfaces;
using TastyTuckTakeaway.Core.Models;

namespace TastyTuckTakeaway.Core.Services
{
    public class PaymentProcessingService : IPaymentProcessingService
    {
        public async Task<bool> ProcessPayment(double amount, string paymentInfo, AppUser user, int orderId)
        {
            var res = await CheckUserValidateDetailsAndTakePayment(user, amount, orderId);
            return res;
        }

        public async Task<bool> CheckUserValidateDetailsAndTakePayment(AppUser user, double amount, int orderId)
        {
            try
            {
                if (user != null)
                {
                    if (user.IsActive && user.BlockAttemps < 3)
                    {
                        if (user.Age > 18)
                        {
                            if (user.PaymentDetails != 0 && user.CardNumber != 0)
                            {
                                var cardNumber = user.CardNumber;
                                if (cardNumber >= 16)
                                {
                                    var encryptedCardNumber = EncryptCardNumber(cardNumber.ToString());

                                    if (encryptedCardNumber != null && encryptedCardNumber.Length > 0)
                                    {
                                        var paymentProviderApiCall = await TakePaymentViaExternalProvider(amount, user.PaymentDetails.ToString());

                                        if (paymentProviderApiCall)
                                        {
                                            return true;
                                        }
                                        else
                                        {
                                            throw new Exception("Invalid card details.");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Error encrypting card details.");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Invalid card number length.");
                                }
                            }
                            else
                            {
                                throw new Exception("Invalid card number.");
                            }
                        }
                        else
                        {
                            throw new Exception("User does not have valid payment details.");
                        }
                    }
                    else
                    {
                        throw new Exception("User account is not active.");
                    }
                }
                else
                {
                    throw new Exception("Invalid user.");
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                throw;
            }
        }

        private string EncryptCardNumber(string cardNumber)
        {
            // Encryption logic
            return "encryptedCardNumber";
        }

        private void LogException(Exception ex)
        {
            // Logging logic
        }


        private async Task<bool> TakePaymentViaExternalProvider(double amount, string paymentInfo)
        {
            await Task.Delay(2500);
            return await Task.FromResult(true);
        }
    }
}