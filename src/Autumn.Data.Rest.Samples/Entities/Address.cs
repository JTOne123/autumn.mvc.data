using MongoDB.Bson.Serialization.Attributes;

namespace Autumn.Data.Rest.Samples.Entities
{
    public class Address
    {
        [BsonElement("street")]
        public string Street { get; set; }

        [BsonElement("suite")]
        public string Suite { get; set; }

        [BsonElement("city")]
        public string City { get; set; }

        [BsonElement("zipcode")]
        public string ZipCode { get; set; }

    }
}