using System;

namespace Autumn.Data.Mvc.Models
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EntityAttribute : Attribute
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}