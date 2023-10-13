using TastyTuckTakeaway.Core.Enums;
using TastyTuckTakeaway.Core.Helpers;
using TastyTuckTakeaway.Core.Models;
using TastyTuckTakeaway.Core.Models.EmailResultTokens;
using TastyTuckTakeaway.Core.Services;

namespace TastyTuckTakeaway.App
{
    public class App
    {
        private readonly IRestaurantService _restaurant;
        private readonly IEmailService _emailService;
        private readonly IPaymentProcessingService _paymentProcessingService;
        private readonly IPasscodeManager _passcodeManager;
        private readonly IPaymentManager _paymentManager;
        private readonly IAddressManager _addressManager;

        public App(IRestaurantService restaurant, IEmailService emailService, IPaymentProcessingService paymentProcessingService,
            IPasscodeManager passcodeManager, IPaymentManager paymentManager, IAddressManager addressManager)
        {
            _restaurant = restaurant;
            _emailService = emailService;
            _paymentProcessingService = paymentProcessingService;
            _passcodeManager = passcodeManager;
            _paymentManager = paymentManager;
            _addressManager = addressManager;
        }

        public void RunAsync()
        {
            GreetCustomer();

            DisplayPreOrderingInstructions();
            PreOrderingOptionsScreen();

            DisplayOrderingInstructions();
            OrderingOptionsScreen();

            var emailResultToken = SendVerificationEmail();
            while (!ConfirmOtp(emailResultToken!.OTP))
            {
                emailResultToken = RetryOtpVerification(emailResultToken);
                if (emailResultToken is null)
                {
                    ExitApplication(1);
                }
            }

            DisplayAddressInstructions();

            var postcode = GetCustomerPostcode();
            CheckValidAddressInput(postcode);

            var houseNumber = GetCustomerHouseNumber();
            CheckValidAddressInput(houseNumber);

            var streetName = "Princes Street";
            AddAddressToOrder(houseNumber, streetName, postcode);

            if (ObtainPayment())
            {
                FullSuccessMessage();
                SendOrderConfirmationEmail(emailResultToken.EmailAddress);
            }
            else
            {
                ExitApplication(1);
            }

            FinalMessage();
            return;
        }

        private void CheckValidAddressInput(string addressInput)
        {
            if (String.IsNullOrEmpty(addressInput))
            {
                MaxTriesExceededMessage();
                ExitApplication(1);
            }
        }

        private static void MaxTriesExceededMessage()
        {
            Console.WriteLine("Max number of attempts exceeded, Exiting...");
        }

        private void ClearBasket()
        {
            _restaurant.ClearBasket();
        }

        private static void FullSuccessMessage()
        {
            Console.WriteLine("Yay!! Your order was successful! Please allow 40-60 minutes for your order to arrive");
            Console.WriteLine("A confirmation email will be sent with your order details");
        }

        private static void FinalMessage()
        {
            Console.WriteLine("Thank you for choosing Tasty Tuck, we hope you enjoy your meal!");
        }

        private void ExitApplication(int errorCode)
        {
            Environment.Exit(errorCode);
        }

        private bool ObtainPayment()
        {
            PaymentInstructions();
            var paymentAttempts = 0;

            while (paymentAttempts < 3)
            {
                var paymentInfo = GetPaymentInfo();

                if (_paymentManager.IsValidPaymentInfo(paymentInfo))
                {
                    var paymentResult = TakeCustomerPayment(paymentInfo!).GetAwaiter().GetResult();

                    if (paymentResult)
                    {
                        return paymentResult;
                    }
                    else
                    {
                        Console.WriteLine("Payment declined. Please try again");
                        paymentAttempts++;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid payment information. Please try again");
                    paymentAttempts++;
                }
            }

            MaxTriesExceededMessage();
            return false;
        }

        private static void PaymentInstructions()
        {
            Console.WriteLine("Please enter your 16 digit card number, expiry date and cvc number - these should be separated by a hash");
            Console.WriteLine("For example: 1234567890123456#0124#123");
        }

        private string? GetPaymentInfo()
        {
            Console.Write("Enter your payment card information: ");
            return GetUserInput();
        }

        private static void GreetCustomer()
        {
            Console.WriteLine("Welcome to Tasty Tuck. Please follow the instructions below to place your order and experience the best Chinese food that Bedford has to offer!");
        }

        private static void DisplayPreOrderingInstructions()
        {
            Console.WriteLine("Type 'm' to see the menu, 'o' to begin an order or 'c' to checkout (after you have items added to your order)");
            Console.WriteLine("Type 'exit' at any time to close the application");
        }

        private static void DisplayOrderingInstructions()
        {
            Console.WriteLine("Type 'add', 'rm' or 'edit' followed by the number of the item on the menu and the quantity");
            Console.WriteLine("For example: 'add 1 2' will add 2 portions of spring rolls to your order");
            Console.WriteLine("Omitting a number on the end of the add command will add 1 portion of the item");
            Console.WriteLine("When removing an item, just include the item number. For example: 'rm 1' removes spring rolls from your order");
            Console.WriteLine("Type 'b' to see the items in your order, 'm' to see the menu, 'i' to see the instructions again or 'c' to checkout");
        }

        private static void DisplayAddressInstructions()
        {
            Console.WriteLine("Please enter your postcode and house number");
        }

        private string GetCustomerPostcode()
        {
            bool validPostcode = false;
            int invalidEntries = 0;

            while (!validPostcode && invalidEntries < 4)
            {
                Console.Write("Enter your postcode: ");
                var postcode = GetUserInput()!;
                if (_addressManager.IsValidUKPostcode(postcode))
                {
                    validPostcode = true;
                    return postcode;
                }
                else
                {
                    Console.WriteLine("Invalid postcode, please try again");
                    invalidEntries++;
                }
            }
            return string.Empty;
        }

        private string GetCustomerHouseNumber()
        {
            bool validHouseNum = false;
            int invalidEntries = 0;

            while (!validHouseNum && invalidEntries < 4)
            {
                Console.Write("Enter your house number: ");
                var houseNum = GetUserInput()!;
                if (_addressManager.IsValidHouseNumber(houseNum))
                {
                    return houseNum;
                }
                else
                {
                    Console.WriteLine("Invalid house number, please try again");
                    invalidEntries++;
                }
            }
            return string.Empty;
        }

        private async Task<bool> TakeCustomerPayment(string paymentInfo)
        {
            return await _paymentProcessingService.ProcessPayment(_restaurant.CalculateTotalOrderCost(), paymentInfo);
        }

        private void PreOrderingOptionsScreen()
        {
            bool orderInProgress = false;

            while (!orderInProgress)
            {
                Console.Write("Enter your choice: ");
                var choice = GetUserInput()!;

                PreOrderingOptions(choice, out orderInProgress);
            }
        }

        private void PreOrderingOptions(string choice, out bool orderInProgress)
        {
            orderInProgress = false;
            switch (choice.ToLower())
            {
                case "m":
                    _restaurant.ShowMenu();
                    break;
                case "o":
                    orderInProgress = true;
                    Console.WriteLine("Order started");
                    break;
                case "c":
                    Console.WriteLine("No items in your order yet, type 'o' to start an order");
                    break;
                default:
                    Console.WriteLine("Invalid choice, please try again");
                    break;
            }
            Console.WriteLine("-----------------------------------------------------------------------");
        }

        private void OrderingOptionsScreen()
        {
            bool orderCompleted = false;
            while (!orderCompleted)
            {
                Console.Write("Enter your choice: ");
                var choice = GetUserInput()!;
                OrderingOptions(choice, out orderCompleted);
            }
        }

        private void OrderingOptions(string choice, out bool orderCompleted)
        {
            orderCompleted = false;
            var inputParts = choice.Split(' ');
            var command = inputParts[0];
            var itemId = 0;
            var quantity = 1;

            if (inputParts.Length > 1)
            {
                itemId = int.Parse(inputParts[1]);
                if (inputParts.Length > 2)
                {
                    quantity = int.Parse(inputParts[2]);
                }
            }

            switch (command.ToLower())
            {
                case "m":
                    _restaurant.ShowMenu();
                    break;
                case "i":
                    DisplayOrderingInstructions();
                    break;
                case "add":
                    var addSuccess = _restaurant.AddItemsToOrder(itemId, quantity);
                    if (addSuccess) Console.WriteLine($"{quantity} x {_restaurant.GetMenuItemById(itemId)!.Name} added to order");
                    break;
                case "rm":
                    var removeSuccess = _restaurant.RemoveItemFromOrder(itemId);
                    if (removeSuccess) Console.WriteLine($"{_restaurant.GetMenuItemById(itemId)!.Name} removed from order");
                    break;
                case "edit":
                    var editSuccess = _restaurant.EditItemInOrder(itemId, quantity);
                    if (editSuccess) Console.WriteLine($"{quantity} x {_restaurant.GetMenuItemById(itemId)!.Name} now in your order");
                    break;
                case "b":
                    Console.WriteLine("***BASKET***");
                    if (_restaurant.IsBasketEmpty()) Console.WriteLine("Basket is currently empty");
                    _restaurant.PrintBasket();
                    Console.WriteLine($"Total: £{_restaurant.CalculateTotalOrderCost()}");
                    break;
                case "c":
                    if (_restaurant.FinaliseOrder()) orderCompleted = true;
                    break;
                case "clear":
                    ClearBasket();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again");
                    break;
            }
            Console.WriteLine("-----------------------------------------------------------------------");
        }

        private VerificationEmailResultToken? SendVerificationEmail(string emailAddress = "")
        {
            var customerEmailAddress = String.IsNullOrEmpty(emailAddress) ? GetCustomerEmailAddress() : emailAddress;
            if (_emailService.IsValidEmail(customerEmailAddress))
            {
                return SendEmail<VerificationEmailResultToken>(customerEmailAddress, EmailTypes.Verification);
            }
            else
            {
                Console.WriteLine("Invalid email address. Please enter a valid email address.");
            }
            return null;
        }

        private OrderConfirmationEmailResultToken? SendOrderConfirmationEmail(string emailAddress)
        {
            return SendEmail<OrderConfirmationEmailResultToken>(emailAddress, EmailTypes.OrderConfirmation, _restaurant.GetOrderNumber(), _restaurant.GetBasketItems());
        }

        private TEmailResultToken? SendEmail<TEmailResultToken>(string emailAddress, EmailTypes emailType, int orderNumber = 0, IEnumerable<OrderItem>? orderItems = null)
            where TEmailResultToken : EmailResultTokenBase
        {
            var emailSentToken = _emailService.SendEmailAsync(emailAddress, emailType, orderNumber, orderItems).GetAwaiter().GetResult();
            if (emailSentToken is not null && emailSentToken.Success)
            {
                return (TEmailResultToken)emailSentToken;
            }
            else
            {
                Console.WriteLine($"{emailType} email was not sent, application error");
            }
            return null;
        }

        private string GetCustomerEmailAddress()
        {
            Console.WriteLine("An email will be sent to you with an OTP for verification purposes");
            Console.Write("Please enter your email address: ");
            var emailAddress = GetUserInput()!;
            return emailAddress;
        }

        private bool ConfirmOtp(string otp)
        {
            Console.Write("Please enter the OTP that was sent to your email address: ");
            var customerOtp = GetUserInput()!;
            if (IsOTPMatch(otp, customerOtp))
            {
                Console.WriteLine("Your email has been confirmed!");
                return true;
            }
            else
            {
                Console.WriteLine("The OTP you entered was incorrect");
                return false;
            }
        }

        private VerificationEmailResultToken? RetryOtpVerification(EmailResultTokenBase initialToken)
        {
            Console.Write("Would you like to request another OTP? (Y/N): ");
            var retryChoice = GetUserInput()?.ToLower();
            if (retryChoice == "y")
            {
                return SendVerificationEmail(initialToken.EmailAddress);
            }
            else
            {
                Console.WriteLine("Order confirmation failed. Exiting...");
                return null;
            }
        }

        private bool IsOTPMatch(string otp, string customerOtp)
        {
            return _passcodeManager.VerifyPasscode(otp, customerOtp);
        }

        private void AddAddressToOrder(string houseNumber, string streetName, string postcode)
        {
            _restaurant.AddAddressToOrder(houseNumber, streetName, postcode);
        }

        public string? GetUserInput()
        {
            var userInput = Console.ReadLine()?.Trim().ToLower();

            if (userInput == "exit")
            {
                ExitApplication(0);
            }

            return userInput;
        }
    }
}