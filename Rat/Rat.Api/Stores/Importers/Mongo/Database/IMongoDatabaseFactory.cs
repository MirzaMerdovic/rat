using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.Stores.Importers.Mongo.Database
{
    public interface IMongoDatabaseFactory
    {
        Task<IMongoDatabase> Get(CancellationToken cancellationToken);
    }
}