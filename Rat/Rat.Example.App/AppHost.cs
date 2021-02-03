using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rat.Clients;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Example.App
{
    public class AppHost : IHostedService
    {
        private readonly AppHostOptions _options;
        private readonly IRatClient _rat;
        private readonly ILogger _logger;

        public AppHost(IOptionsMonitor<AppHostOptions> options, IRatClient rat, ILogger<AppHost> logger)
        {
            _options = options.CurrentValue ?? throw new ArgumentNullException(nameof(options));
            _rat = rat ?? throw new ArgumentNullException(nameof(rat));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _rat.RegisterClient(new Data.ClientRegistrationEntry { Name = "Example.App", Endpoint = _options.BaseUrl }, cancellationToken);

            while(true)
            {
                var entry = await _rat.GetValue<string>(_options.ConfigurationKeyToRetrieve, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation($"Value == {entry}");

                await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);

                if (entry == null)
                    throw new ArgumentNullException("Configuration entry is null");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}