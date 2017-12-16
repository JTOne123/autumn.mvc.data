using System;
using Autumn.Mvc.Data.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Mvc.Data.MongoDB.Samples.Models
{
    public abstract class AbstractEntity
    {
        [BsonId]
        [Id()]
        [Ignore()]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [CreatedBy]
        [BsonElement("createdBy")]
        public string CreatedBy { get; set; }
        
        [LastModifiedBy]
        [BsonElement("modifiedBy")]
        public string LastModifiedBy { get; set; }
        
        [CreatedDate]
        [BsonElement("createdDate")]
        public DateTime CreatedDate { get; set; }
        
        [LastModifiedDate]
        [BsonElement("modifiedDate")]
        public DateTime? LastModifiedDate { get; set; }
    }
}