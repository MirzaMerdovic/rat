using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Rat.Data
{
    public class ClientRegistrationEntry
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string Endpoint { get; set; }
    }
}