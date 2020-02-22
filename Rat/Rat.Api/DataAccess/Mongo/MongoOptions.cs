namespace Rat.Api.DataAccess.Mongo
{
    public sealed class MongoConnectionOptions
    {
        public string Name { get; set; }

        public string Url { get; set; }
    }

    public sealed class MongoDatabaseOptions
    {
        public string Name { get; set; }
    }

    public abstract class MongoCollectionOptions
    {
        public string Name { get; set; }
    }

    public sealed class ConfigurationMongoCollectionOptions : MongoCollectionOptions
    {
    }

    public sealed class ClientMongoCollectionOptions : MongoCollectionOptions
    {
    }
}