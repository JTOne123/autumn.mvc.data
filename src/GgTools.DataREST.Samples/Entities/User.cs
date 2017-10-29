using System.Threading;
using MongoDB.Bson.Serialization.Attributes;
using WebApplication1.Repositories;

namespace WebApplication1.Entities
{
    [BsonIgnoreExtraElements]
    [BsonCollection("users")]
    public class User : AbstractEntity
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
        public Address Adresse { get; set; }
        
        [BsonElement("count")]
        public int? Count { get; set; }
    }
}