using Microsoft.Extensions.DependencyInjection;
using TastyTuckTakeaway.Core.Helpers;
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
    }
}