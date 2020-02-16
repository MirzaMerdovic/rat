using Microsoft.Extensions.Logging;
using Rat.Data;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores
{
    public class ConfigurationStore : IConfigurationStore
    {
        private readonly ConcurrentDictionary<string, ConfigurationEntry> _entries;
        private readonly ILogger _logger;

        public ConfigurationStore(ConcurrentDictionary<string, ConfigurationEntry> entries, ILogger<ConfigurationStore> logger)
        {
            _entries = entries ?? new ConcurrentDictionary<string, ConfigurationEntry>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<ConfigurationEntry> GetEntry(string key, CancellationToken cancellation)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            key = key.ToUpperInvariant();

            if (_entries.ContainsKey(key.ToUpperInvariant()))
                return Task.FromResult(_entries[key]);

            _logger.LogInformation($"Configuration entry for key: {key} was not found.");

            return Task.FromResult<ConfigurationEntry>(null);
        }
    }
}