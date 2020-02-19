using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rat.Providers.Configuration;
using Rat.Providers.Resiliency;
using System.IO;

namespace Rat.Example.App
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configurator =>
                {
                    configurator.SetBasePath(Directory.GetCurrentDirectory());
                    configurator.AddJsonFile("appsettings.json", optional: true);
                    configurator.AddCommandLine(args);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<RatClientOptions>(context.Configuration.GetSection(nameof(RatClientOptions)));

                    services.AddHttpClient();
                    services.AddMemoryCache();
                    services.AddTransient<IRetryProvider, RetryProvider>();
                    services.AddSingleton<IRatClient, RatClient>();
                    services.AddHostedService<AppHost>();
                });
    }
}
