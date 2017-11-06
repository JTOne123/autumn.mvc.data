using Autumn.Data.Mvc.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Data.Mvc.Samples.Models
{
    public abstract class AbstractEntity
    {
        [BsonId]
        [Id]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}