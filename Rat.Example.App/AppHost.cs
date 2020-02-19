using Microsoft.Extensions.Hosting;
using Rat.Data;
using Rat.Providers.Configuration;
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
            var entry = await _rat.GetValue<string>("A1", cancellationToken).ConfigureAwait(false);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
