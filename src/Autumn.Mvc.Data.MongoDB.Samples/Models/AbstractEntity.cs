using Autumn.Mvc.Data.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Mvc.Data.MongoDB.Samples.Models
{
    public abstract class AbstractEntity
    {
        [BsonId]
        [AutumnKey()]
        [AutumnIgnore(AutumnIgnoreType.All)]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}