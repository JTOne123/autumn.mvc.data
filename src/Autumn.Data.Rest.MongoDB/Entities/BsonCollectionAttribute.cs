using System;

namespace Autumn.Data.Rest.MongoDB.Entities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BsonCollectionAttribute :Attribute
    {
        public string CollectionName { get;}

        public BsonCollectionAttribute(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}