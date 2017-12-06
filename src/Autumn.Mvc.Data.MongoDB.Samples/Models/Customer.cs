using System.ComponentModel.DataAnnotations;
using Autumn.Mvc.Data.Annotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Autumn.Mvc.Data.MongoDB.Samples.Models
{
    [BsonIgnoreExtraElements]
    [Entity(Name = "customers",Version = "v1")]
    public class Customer : AbstractEntity
    {    
        [Required]
        [BsonElement("name")]
        public string Name { get; set; }

        [Required]
        [BsonElement("username")]
        public string UserName { get; set; }
    }
    
    [BsonIgnoreExtraElements]
    [Entity(Name = "customers",Version = "v2")]
    public class CustomerV2 : Customer
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
    [Entity(Name = "customers", Version = "v3")]
    public class CustomerV3 : Customer
    {

        [Ignore(Insertable = true)]
        [BsonElement("active")]
        public bool? Active { get; set; }
    }
    
    
    
    [BsonIgnoreExtraElements]
    [Entity(Name = "customers", Version = "v4")]
    public class CustomerV4 : AbstractEntity
    {

        [BsonElement("test")]
        public string Test { get; set; }
    }
}