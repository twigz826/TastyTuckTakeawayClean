using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using TastyTuckTakeaway.Core.Helpers;
using TastyTuckTakeaway.Core.Interfaces;
using TastyTuckTakeaway.Core.Models;
using TastyTuckTakeaway.Core.Models.EmailResultTokens;
using File = System.IO.File;

namespace TastyTuckTakeaway.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly IPasscodeManager _pM;

        public EmailService(IPasscodeManager passcodeManager)
        {
            _pM = passcodeManager;
        }

        public async Task<EmailResultTokenBase?> SendEmailAsync(string emailAddress, string emailType, int orderNumber = 0, IEnumerable<OrderItem>? orderItems = null)
        {
            var se = Environment.GetEnvironmentVariable("DTSCLEANCODEDEMO_GMAIL_ADDRESS");
            var pw = Environment.GetEnvironmentVariable("DTSCLEANCODEDEMO_GMAIL_PASSWORD");

            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(se, pw),
                EnableSsl = true
            };

            try
            {
                MailMessage message = new MailMessage();

                switch (emailType)
                {
                    case "Verification":
                        var random = new Random();
                        var otp = random.Next(100000, 999999).ToString();
                        message = ComposeVerificationEmail(emailAddress, se, otp);
                        await client.SendMailAsync(message);
                        return new VerificationEmailResultToken(true, emailAddress, otp);

                    case "OrderConfirmation":
                        message = ComposeConfirmationEmail(emailAddress, se, orderNumber, orderItems!);
                        await client.SendMailAsync(message);
                        return new OrderConfirmationEmailResultToken(true, emailAddress, orderNumber);

                    default:
                        Console.WriteLine("Error occurred, please try again");
                        return null;
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Email failed to send: {ex}");
                return null;
            }
        }

        public bool IsValidEmail(string? em)
        {
            string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            if (em != null)
            {
                return Regex.IsMatch(em, emailPattern);
            }
            return false;
        }

        public MailMessage ComposeVerificationEmail(string ea, string se, string otp)
        {
            var m = new MailMessage
            {
                From = new MailAddress(se),
                Subject = "TastyTuckTakeway - Email Verification",
                Body = File.ReadAllText("verification-email-template.html").Replace("{{OTP}}", otp),
                IsBodyHtml = true
            };

            m.To.Add(new MailAddress(ea));
            return m;
        }

        public MailMessage ComposeConfirmationEmail(string ea, string se, int on, IEnumerable<OrderItem> oi)
        {
            var m = new MailMessage
            {
                From = new MailAddress(se),
                Subject = "TastyTuckTakeway - Order Confirmation",
                Body = File.ReadAllText("orderconfirmation-email-template.html")
                    .Replace("{{OrderNumber}}", on.ToString())
                    .Replace("{{OrderItems}}", AddOrderItemsToHtml(oi)),
                IsBodyHtml = true
            };

            m.To.Add(new MailAddress(ea));
            return m;
        }

        public static string AddOrderItemsToHtml(IEnumerable<OrderItem> items)
        {
            var oih = "<ul style='list-style-type: none; padding-left: 0;'>";
            foreach (var oi in items)
            {
                oih += $"<li>{oi.Quantity} x {oi.MenuItem.Name} - {oi.Quantity * oi.MenuItem.Price}</li>";
            }
            oih += "</ul>";

            var t = items.Sum(item => item.Quantity * item.MenuItem.Price);
            return oih + $"<p><strong>Total: £{t}</strong></p>";
        }

        public string GenerateRandomOTP()
        {
            return _pM.GenerateOTP();
        }
    }
}