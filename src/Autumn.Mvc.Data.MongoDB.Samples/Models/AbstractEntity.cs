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
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [CreatedBy]
        [Ignore(Updatable = true, Insertable = false)]
        public string CreatedBy { get; set; }
        
        [LastModifiedBy]
        public string LastModifiedBy { get; set; }
        
        [CreatedDate]
        public DateTime CreatedDate { get; set; }
        
        [LastModifiedDate]
        public DateTime? LastModifiedDate { get; set; }
    }
}