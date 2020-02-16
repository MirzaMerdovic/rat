using Microsoft.Extensions.Logging;
using Rat.Data;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Providers.Client
{
    public interface IClientRegistrationProvider
    {
        Task Register(ClientRegistrationEntry entry, CancellationToken cancellation);

        Task Notify(ClientRegistrationEntry entry, CancellationToken cancellation);
    }

    public class ClientRegistrationProvider : IClientRegistrationProvider
    {
        private readonly ConcurrentDictionary<string, ClientRegistrationEntry> _entries = new ConcurrentDictionary<string, ClientRegistrationEntry>();

        private readonly ILogger _logger;

        public ClientRegistrationProvider(ILogger<ClientRegistrationProvider> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Register(ClientRegistrationEntry entry, CancellationToken cancellation)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            _entries[entry.Name.Trim().ToUpperInvariant()] = entry;

            return Task.CompletedTask;
        }

        public Task Notify(ClientRegistrationEntry entry, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}
