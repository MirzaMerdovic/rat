using Microsoft.Extensions.Hosting;
using Rat.Clients;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Example.App
{
    public class AppHost : IHostedService
    {
        private readonly IRatClient _rat;

        public AppHost(IRatClient rat)
        {
            _rat = rat ?? throw new ArgumentNullException(nameof(rat));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _rat.RegisterClient(new Data.ClientRegistrationEntry { Name = "Example.App", Endpoint = "http://localhost:5441" }, cancellationToken);

            var entry = await _rat.GetValue<string>("A1", cancellationToken).ConfigureAwait(false);

            if (entry == null)
                throw new ArgumentNullException("Configuration entry is null");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}