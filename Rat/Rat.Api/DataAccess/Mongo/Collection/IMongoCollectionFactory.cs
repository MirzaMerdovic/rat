using MongoDB.Driver;
using System.Threading;
using System.Threading.Tasks;

namespace Rat.Api.DataAccess.Mongo.Collection
{
    public interface IMongoCollectionFactory
    {
        Task<IMongoCollection<T>> Get<T>(CancellationToken cancellationToken);
    }

    public interface IClientMongoCollectionFactory : IMongoCollectionFactory
    {
    }

    public interface IConfigurationMongoCollectionFactory : IMongoCollectionFactory
    {
    }
}