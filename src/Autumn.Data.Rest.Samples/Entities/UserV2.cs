using Autumn.Data.Rest.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Autumn.Data.Rest.Samples.Repositories;
using Newtonsoft.Json;

namespace Autumn.Data.Rest.Samples.Entities
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