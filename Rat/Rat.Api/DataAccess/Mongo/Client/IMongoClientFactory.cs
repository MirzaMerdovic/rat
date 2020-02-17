using MongoDB.Driver;

namespace Rat.Api.DataAccess.Mongo.Client
{
    public interface IMongoClientFactory
    {
        IMongoClient Create();
    }
}