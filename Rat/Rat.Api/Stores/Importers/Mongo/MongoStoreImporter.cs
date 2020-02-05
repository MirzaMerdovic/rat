using Rat.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores.Importers.Mongo
{
    public class MongoStoreImporter : IStoreImporter
    {
        public Task<IReadOnlyCollection<ConfigurationEntry>> Import(CancellationToken cancellation)
        {
            IReadOnlyCollection<ConfigurationEntry> imports = new List<ConfigurationEntry>
            {
                new ConfigurationEntry { Key = "A2", Value = "Hallon Mongo", Expiration = TimeSpan.FromSeconds(30) },
                new ConfigurationEntry { Key = "B1", Value = "Ahoj!", Expiration = TimeSpan.FromSeconds(90) }
            };

            return Task.FromResult(imports);
        }
    }
}