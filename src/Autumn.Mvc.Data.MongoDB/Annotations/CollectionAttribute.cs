using System;

namespace Autumn.Mvc.Data.MongoDB.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CollectionAttribute : Attribute
    {
        public string Name { get; set; }
    }
}