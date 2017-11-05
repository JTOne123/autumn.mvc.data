using Autumn.Data.Rest.Entities;
using Autumn.Data.Rest.Samples.Repositories;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Data.Rest.Samples.Entities
{
    [BsonIgnoreExtraElements]
    [Entity(Name = "articles")]
    public class Article : AbstractEntity
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("article")]
        public string Content { get; set; }
    }
}