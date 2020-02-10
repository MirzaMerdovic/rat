using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Rat.Data
{
    public class MongoConfigurationEntry : ConfigurationEntry
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}