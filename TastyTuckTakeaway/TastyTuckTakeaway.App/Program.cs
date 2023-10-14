using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TastyTuckTakeaway.App;
using TastyTuckTakeaway.App.Configuration;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    await services.GetRequiredService<App>().RunAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Application failed: {ex.Message}");
}

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddCoreDependencies();
            services.AddLogging();
            services.AddSingleton<App>();
        });
}
