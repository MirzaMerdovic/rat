using Microsoft.Extensions.Logging;
using Rat.Api.Importers;
using Rat.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores
{
    public interface IConfigurationStore
    {
        Task Load(CancellationToken cancellation);

        Task<ConfigurationEntry> GetEntry(string key, CancellationToken cancellation);
    }

    public class ConfigurationStore : IConfigurationStore
    {
        private readonly ConcurrentDictionary<string, ConfigurationEntry> _entries = new ConcurrentDictionary<string, ConfigurationEntry>();

        private readonly ILogger _logger;
        private readonly IEnumerable<IStoreImporter> _importers;

        public ConfigurationStore(IEnumerable<IStoreImporter> importers, ILogger<ConfigurationStore> logger)
        {
            _importers = importers ?? throw new ArgumentNullException(nameof(importers));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Load(CancellationToken cancellation)
        {
            foreach (var importer in _importers)
            {
                var store = await importer.Import(cancellation).ConfigureAwait(false);

                foreach (var item in store)
                {
                    if (string.IsNullOrWhiteSpace(item.Key))
                        throw new ArgumentException($"FileStore: {importer.Type} has and entry without key.");

                    _entries[item.Key.Trim().ToUpperInvariant()] = item;
                }
            }
        }

        public Task<ConfigurationEntry> GetEntry(string key, CancellationToken cancellation)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            key = key.Trim().ToUpperInvariant();

            if (_entries.ContainsKey(key.ToUpperInvariant()))
                return Task.FromResult(_entries[key]);

            _logger.LogInformation($"Configuration entry for key: {key} was not found.");

            return Task.FromResult<ConfigurationEntry>(null);
        }
    }
}