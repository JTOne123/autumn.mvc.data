using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Repositories
{
    public abstract class AbstractEntity
    {
        [BsonElement("_id")]
        [BsonId]
        public ObjectId Id { get; set; }
    }
}