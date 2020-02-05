using Rat.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores.Importers.Environment
{
    public class EnvironmentStoreImporter : IStoreImporter
    {
        public Task<IReadOnlyCollection<ConfigurationEntry>> Import(CancellationToken cancellation)
        {
            IReadOnlyCollection<ConfigurationEntry> imports = new List<ConfigurationEntry> { new ConfigurationEntry { Key = "A1", Value = "Alloo", Expiration = TimeSpan.FromSeconds(30) } };

            return Task.FromResult(imports);
        }
    }
}