using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TastyTuckTakeaway.Core.Configuration;
using TastyTuckTakeaway.Core.Helpers;
using TastyTuckTakeaway.Core.Interfaces;
using TastyTuckTakeaway.Core.Models;
using TastyTuckTakeaway.Core.Services;

namespace TastyTuckTakeaway.App.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IRestaurantService, RestaurantService>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IPaymentProcessingService, PaymentProcessingService>();
            services.AddSingleton<IPaymentManager, PaymentManager>();
            services.AddSingleton<IAddressManager, AddressManager>();
            services.AddSingleton<IMenu, Menu>();
            services.AddSingleton<Order>();
            services.AddSingleton<IPasscodeManager, PasscodeManager>();

            return services;
        }

        public static IServiceCollection AddEmailSettings(this IServiceCollection services, HostBuilderContext context)
        {
            var emailSettingsSection = context.Configuration.GetSection("EmailSettings");

            var emailSettings = new EmailSettings
            {
                SmtpServer = emailSettingsSection["SmtpServer"]!,
                Port = int.Parse(emailSettingsSection["Port"]!),
                Username = Environment.GetEnvironmentVariable("DTSCLEANCODEDEMO_GMAIL_ADDRESS")!,
                Password = Environment.GetEnvironmentVariable("DTSCLEANCODEDEMO_GMAIL_PASSWORD")!
            };

            services.AddSingleton(emailSettings);
            return services;
        }
    }
}