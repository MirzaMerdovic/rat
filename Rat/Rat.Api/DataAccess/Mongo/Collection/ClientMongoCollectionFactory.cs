using Microsoft.Extensions.Options;
using Rat.Api.DataAccess.Mongo.Database;

namespace Rat.Api.DataAccess.Mongo.Collection
{
    public class ClientMongoCollectionFactory : MongoCollectionFactory<ClientMongoCollectionOptions>, IClientMongoCollectionFactory
    {
        public ClientMongoCollectionFactory(IOptions<ClientMongoCollectionOptions> options, IMongoDatabaseFactory databaseFactory)
            : base(options, databaseFactory)
        {
        }
    }
}