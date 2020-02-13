using Rat.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores.Importers.SqlServer
{
    public class SqlServerStoreImporter : IStoreImporter
    {
        public int Rank => 0;

        public string Type => "SqlServer";

        public Task<IEnumerable<ConfigurationEntry>> Import(CancellationToken cancellation)
        {
            return Task.FromResult(Enumerable.Empty<ConfigurationEntry>());
        }
    }
}