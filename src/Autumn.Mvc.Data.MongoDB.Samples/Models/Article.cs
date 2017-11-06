using Autumn.Mvc.Data.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Mvc.Data.MongoDB.Samples.Models
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