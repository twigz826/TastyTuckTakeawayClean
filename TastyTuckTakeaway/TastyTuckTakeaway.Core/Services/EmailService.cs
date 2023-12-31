﻿using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using TastyTuckTakeaway.Core.Configuration;
using TastyTuckTakeaway.Core.Enums;
using TastyTuckTakeaway.Core.Helpers;
using TastyTuckTakeaway.Core.Models;
using TastyTuckTakeaway.Core.Models.EmailResultTokens;
using File = System.IO.File;

namespace TastyTuckTakeaway.Core.Services
{
    public interface IEmailService
    {
        Task<EmailResultTokenBase?> SendEmailAsync(string emailAddress, EmailTypes emailType, int orderNumber = 0, IEnumerable<OrderItem>? orderItems = null);

        bool IsValidEmail(string emailAddress);
    }

    public class EmailService : IEmailService
    {
        private readonly IPasscodeManager _passcodeManager;
        private readonly EmailSettings _emailSettings;

        public EmailService(IPasscodeManager passcodeManager, EmailSettings emailSettings)
        {
            _passcodeManager = passcodeManager;
            _emailSettings = emailSettings;
        }

        public async Task<EmailResultTokenBase?> SendEmailAsync(string emailAddress, EmailTypes emailType, int orderNumber = 0, IEnumerable<OrderItem>? orderItems = null)
        {
            var sender = _emailSettings.Username;
            var pw = _emailSettings.Password;

            var client = SetupSmtpClient(sender, pw);

            try
            {
                MailMessage message = new();
                switch (emailType)
                {
                    case EmailTypes.Verification:
                        var otp = GenerateRandomOTP();
                        message = ComposeVerificationEmail(emailAddress, sender, otp);
                        await client.SendMailAsync(message);
                        return new VerificationEmailResultToken(true, emailAddress, otp);

                    case EmailTypes.OrderConfirmation:
                        message = ComposeConfirmationEmail(emailAddress, sender, orderNumber, orderItems!);
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

        public bool IsValidEmail(string? emailAddress)
        {
            string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            if (emailAddress is not null)
            {
                return Regex.IsMatch(emailAddress, emailPattern);
            }
            return false;
        }

        private static MailMessage ComposeVerificationEmail(string emailAddress, string sender, string otp)
        {
            var message = new MailMessage
            {
                From = new MailAddress(sender),
                Subject = "TastyTuckTakeway - Email Verification",
                Body = GenerateVerificationEmailBodyHtml(otp),
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(emailAddress));
            return message;
        }

        private static MailMessage ComposeConfirmationEmail(string emailAddress, string sender, int orderNumber, IEnumerable<OrderItem> orderItems)
        {
            var message = new MailMessage
            {
                From = new MailAddress(sender),
                Subject = "TastyTuckTakeway - Order Confirmation",
                Body = GenerateConfirmationEmailBodyHtml(orderNumber, orderItems),
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(emailAddress));
            return message;
        }

        private static string GenerateVerificationEmailBodyHtml(string otp)
        {
            var htmlTemplate = File.ReadAllText("verification-email-template.html");
            var emailBody = htmlTemplate.Replace("{{OTP}}", otp);
            return emailBody;
        }

        private static string GenerateConfirmationEmailBodyHtml(int orderNumber, IEnumerable<OrderItem> orderItems)
        {
            var htmlTemplate = File.ReadAllText("orderconfirmation-email-template.html");
            var emailBody = htmlTemplate.Replace("{{OrderNumber}}", orderNumber.ToString());
            emailBody = AddOrderItemsToHtml(emailBody, orderItems);

            return emailBody;
        }

        private static string AddOrderItemsToHtml(string emailBody, IEnumerable<OrderItem> orderItems)
        {
            var orderItemsHtml = "<ul style='list-style-type: none; padding-left: 0;'>";
            foreach (var orderItem in orderItems)
            {
                orderItemsHtml += $"<li>{orderItem.Quantity} x {orderItem.MenuItem.Name} - {orderItem.Quantity * orderItem.MenuItem.Price}</li>";
            }
            orderItemsHtml += "</ul>";

            var totalHtml = AddTotalToHtml(orderItems);

            emailBody = emailBody.Replace("{{OrderItems}}", orderItemsHtml + totalHtml);
            return emailBody;
        }

        private static object AddTotalToHtml(IEnumerable<OrderItem> orderItems)
        {
            var total = orderItems.Sum(item => item.Quantity * item.MenuItem.Price);
            return $"<p><strong>Total: £{total}</strong></p>";
        }

        private SmtpClient SetupSmtpClient(string sendingEmailAddress, string password)
        {
            var client = new SmtpClient(_emailSettings.SmtpServer)
            {
                Port = _emailSettings.Port,
                Credentials = new NetworkCredential(sendingEmailAddress, password),
                EnableSsl = true
            };
            return client;
        }

        private string GenerateRandomOTP()
        {
            return _passcodeManager.GenerateOTP();
        }
    }
}