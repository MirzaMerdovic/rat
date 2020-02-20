using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Rat.Clients;
using Rat.Clients.Options;
using Rat.Providers.Resiliency;
using System.IO;
using Rat.Middleware;
using Microsoft.AspNetCore.Builder;

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
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                    builder.ConfigureKestrel((ctx, kso) =>
                    {
                        kso.ListenAnyIP(5441);
                    });
                    builder.Configure(app => app.UseMiddleware<RatMiddleware>());
                })
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