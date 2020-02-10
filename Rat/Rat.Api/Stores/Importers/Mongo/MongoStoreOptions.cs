namespace Rat.Api.Stores.Importers.Mongo
{
    public class MongoStoreOptions
    {
        public bool Enabled { get; set; }
    }

    public sealed class MongoConnectionOptions
    {
        public string Name { get; set; }

        public string Url { get; set; }
    }

    public sealed class MongoDatabaseOptions
    {
        public string Name { get; set; }
    }

    public sealed class MongoCollectionOptions
    {
        public string Name { get; set; }
    }
}