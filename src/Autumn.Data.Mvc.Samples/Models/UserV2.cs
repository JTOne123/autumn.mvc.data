using Autumn.Data.Mvc.Models;
using Autumn.Data.Mvc.Samples.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Autumn.Data.Mvc.Samples.Entities
{
    [BsonIgnoreExtraElements]
    [Entity(Name = "users")]
    public class UserV2 : AbstractEntity
    {
        
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("username")]
        public string UserName { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("address")]
        public Address Address { get; set; }
        
        [JsonIgnore]
        [BsonElement("count")]
        public int Count { get; set; }
    }
}