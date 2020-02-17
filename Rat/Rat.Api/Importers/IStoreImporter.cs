using Rat.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Importers
{
    public interface IStoreImporter
    {
        int Rank { get; }

        string Type { get; }

        Task<IEnumerable<ConfigurationEntry>> Import(CancellationToken cancellation);
    }
}