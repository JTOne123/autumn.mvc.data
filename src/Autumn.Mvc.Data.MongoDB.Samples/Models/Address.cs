using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Mvc.Data.MongoDB.Samples.Models
{
    public class Address
    {
        [BsonElement("street")]
        public string Street { get; set; }

        [BsonElement("state")]
        public string State { get; set; }

        [BsonElement("city")]
        public string City { get; set; }

        [BsonElement("postal_code")]
        public string PostalCode { get; set; }
    
    }
}