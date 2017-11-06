using Autumn.Data.Mvc.Models;
using Autumn.Data.Mvc.Samples.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Data.Rest.Samples.Models
{
    [BsonIgnoreExtraElements]
    [Entity(Name = "users",Version = "v1")]
    public class User : AbstractEntity
    {
        
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("username")]
        public string UserName { get; set; }
    }
}