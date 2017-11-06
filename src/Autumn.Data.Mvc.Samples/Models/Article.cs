using Autumn.Data.Mvc.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Data.Mvc.Samples.Models
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