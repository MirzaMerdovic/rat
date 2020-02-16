using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Rat.Api.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api
{
    /// <summary>
    /// Where all begins.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">Arguments</param>
        /// <returns>A task.</returns>
        public static async Task Main(string[] args)
        {
            using var source = new CancellationTokenSource();

            var builder = CreateHostBuilder(args).Build();
            var store = (IConfigurationStore)builder.Services.GetService(typeof(IConfigurationStore));
            await store.Load(source.Token).ConfigureAwait(false);

            await builder.RunAsync(source.Token).ConfigureAwait(false);
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}