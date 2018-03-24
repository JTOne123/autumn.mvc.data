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
        public string LastName { get; set; }

        [Required]
        public string FirstName { get; set; }
        
        public DateTime? BirthDate { get; set; }
    }

    [BsonIgnoreExtraElements]
    [Entity(Name = "customers", Version = "v2")]
    public class CustomerV2 : Customer
    {

        public string Email { get; set; }

        public Address Address { get; set; }
    }

    [BsonIgnoreExtraElements]
    [Entity(Name = "customers", Version = "v3")]
    public class CustomerV3 : CustomerV2
    {

        [Ignore(Insertable = true)]
        public bool? Active { get; set; }
    }
    
    
    
    [BsonIgnoreExtraElements]
    [Entity(Name = "customers", Version = "v4")]
    public class CustomerV4 : CustomerV3
    {

        public string Account { get; set; }
        
        public double Debit { get; set; }
        
        public double Credit { get; set; }

    }
}