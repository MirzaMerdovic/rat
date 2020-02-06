using Rat.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores.Importers.Mongo
{
    public class MongoStoreImporter : IStoreImporter
    {
        public string Type => "MongoDb";

        public Task<IEnumerable<ConfigurationEntry>> Import(CancellationToken cancellation)
        {
            return Task.FromResult(Enumerable.Empty<ConfigurationEntry>());
        }
    }
}