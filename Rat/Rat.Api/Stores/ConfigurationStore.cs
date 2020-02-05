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

        public ConfigurationStore(ConcurrentDictionary<string, ConfigurationEntry> entries)
        {
            _entries = entries ?? new ConcurrentDictionary<string, ConfigurationEntry>();
        }

        public Task<ConfigurationEntry> GetEntry(string key, CancellationToken cancellation)
        {
            return Task.FromResult(_entries[key]);
        }
    }
}