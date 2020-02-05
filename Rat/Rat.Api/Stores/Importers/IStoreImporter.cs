using Rat.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores
{
    public interface IStoreImporter
    {
        Task<IReadOnlyCollection<ConfigurationEntry>> Import(CancellationToken cancellation);
    }
}