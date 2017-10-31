using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Data.Rest.Samples.Repositories
{
    public abstract class AbstractEntity
    {
        [BsonElement("_id")]
        [BsonId]
        public ObjectId Id { get; set; }
    }
}