using Autumn.Mvc.Data.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Autumn.Mvc.Data.MongoDB.Samples.Models
{
    [BsonIgnoreExtraElements]
    [Entity(Name = "user",Version = "v1")]
    public class User : AbstractEntity
    {
        
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("username")]
        public string UserName { get; set; }
    }
    
    [BsonIgnoreExtraElements]
    [Entity(Name = "user",Version = "v2")]
    public class UserV2 : User
    {

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("address")]
        public Address Address { get; set; }
        
        [JsonIgnore]
        [BsonElement("count")]
        public int Count { get; set; }
    }

    [BsonIgnoreExtraElements]
    [Entity(Name = "user", Version = "v3")]
    public class UserV3 : UserV2
    {

        [BsonElement("active")]
        public bool? Active { get; set; }
    }
    
    
    
    [BsonIgnoreExtraElements]
    [Entity(Name = "user", Version = "v4")]
    public class UserV4 : AbstractEntity
    {

        [BsonElement("test")]
        public string Test { get; set; }
    }
}