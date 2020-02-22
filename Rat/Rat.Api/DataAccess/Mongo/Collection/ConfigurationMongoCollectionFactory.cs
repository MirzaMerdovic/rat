using Microsoft.Extensions.Options;
using Rat.Api.DataAccess.Mongo.Database;

namespace Rat.Api.DataAccess.Mongo.Collection
{
    public class ConfigurationMongoCollectionFactory : MongoCollectionFactory<ConfigurationMongoCollectionOptions>, IConfigurationMongoCollectionFactory
    {
        public ConfigurationMongoCollectionFactory(IOptions<ConfigurationMongoCollectionOptions> options, IMongoDatabaseFactory databaseFactory)
            : base(options, databaseFactory)
        {
        }
    }
}