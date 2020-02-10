using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores.Importers.Mongo.Collection
{
    public interface IMongoCollectionFactory
    {
        Task<IMongoCollection<T>> Get<T>(CancellationToken cancellationToken);
    }
}