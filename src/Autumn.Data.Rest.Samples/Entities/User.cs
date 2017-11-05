using Autumn.Data.Rest.Entities;
using MongoDB.Bson.Serialization.Attributes;
using Autumn.Data.Rest.Samples.Repositories;
using Newtonsoft.Json;

namespace Autumn.Data.Rest.Samples.Entities
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