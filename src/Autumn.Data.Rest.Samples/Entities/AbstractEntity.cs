using System;
using Autumn.Data.Rest.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Data.Rest.Samples.Repositories
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