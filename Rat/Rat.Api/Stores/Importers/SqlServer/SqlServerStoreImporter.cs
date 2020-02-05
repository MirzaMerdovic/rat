using Rat.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores.Importers.SqlServer
{
    public class SqlServerStoreImporter : IStoreImporter
    {
        public Task<IReadOnlyCollection<ConfigurationEntry>> Import(CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }
    }
}