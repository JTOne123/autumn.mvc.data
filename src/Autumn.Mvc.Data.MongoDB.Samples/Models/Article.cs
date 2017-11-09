using System;
using Autumn.Mvc.Data.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Mvc.Data.MongoDB.Samples.Models
{
    [BsonIgnoreExtraElements]
    [Entity(Name = "articles",Version = "v1")]
    public class Article : AbstractEntity
    {
        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }
    }
    
    [BsonIgnoreExtraElements]
    [Entity(Name = "articles",Version = "v2")]
    public class ArticleV2 : Article
    {
        [BsonElement("publish_date")]
        public DateTime? PublishDate { get; set; }
    }
}