using System;

namespace Autumn.Data.Rest.Entities
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EntityAttribute : Attribute
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}