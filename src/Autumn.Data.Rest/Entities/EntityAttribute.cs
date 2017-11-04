using System;

namespace Autumn.Data.Rest.Entities
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityAttribute : Attribute
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}