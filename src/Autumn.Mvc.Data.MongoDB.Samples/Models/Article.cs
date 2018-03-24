using System;
using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Mvc.Data.MongoDB.Samples.Models
{
    [BsonIgnoreExtraElements]
    [Entity(Name = "articles", Version = "v1",Deletable = false,Updatable = false)]
    public class Article : AbstractEntity
    {
        [Required]
        public string Title { get; set; }

        public string Content { get; set; }
    }

    [BsonIgnoreExtraElements]
    [Entity(Name = "articles", Version = "v2",Insertable = false)]
    public class ArticleV2 : Article
    {
        public DateTime PublishDate { get; set; }

        [Range(0, 100)]
        public int Score { get; set; }
    }
}