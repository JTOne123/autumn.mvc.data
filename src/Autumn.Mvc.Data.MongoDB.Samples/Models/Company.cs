using System.Collections.Generic;
using Autumn.Mvc.Data.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Mvc.Data.MongoDB.Samples.Models
{
    [Entity(Name = "companies", Version = "v1")]
    public class Company : AbstractEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("suffixes")]
        public List<string> Suffixe { get; set; }
    }
}