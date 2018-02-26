using System;
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
        [BsonElement("last_name")]
        public string LastName { get; set; }

        [Required]
        [BsonElement("first_name")]
        public string FirstName { get; set; }
        
        [BsonElement("birth_date")]
        public DateTime? BirthDate { get; set; }
    }

    [BsonIgnoreExtraElements]
    [Entity(Name = "customers", Version = "v2")]
    public class CustomerV2 : Customer
    {

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("address")]
        public Address Address { get; set; }
    }

    [BsonIgnoreExtraElements]
    [Entity(Name = "customers", Version = "v3")]
    public class CustomerV3 : CustomerV2
    {

        [Ignore(Insertable = true)]
        [BsonElement("active")]
        public bool? Active { get; set; }
    }
    
    
    
    [BsonIgnoreExtraElements]
    [Entity(Name = "customers", Version = "v4")]
    public class CustomerV4 : CustomerV3
    {

        [BsonElement("account")]
        public string Account { get; set; }
        
        [BsonElement("debit")]
        public double Debit { get; set; }
        
        [BsonElement("credit")]
        public double Credit { get; set; }

    }
}