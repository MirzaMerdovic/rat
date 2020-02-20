using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Rat.Data
{
    public class MongoClientRegistrationEntry : ClientRegistrationEntry
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}