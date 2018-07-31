using System;

namespace Autumn.Mvc.Data.Annotations
{
    /// <summary>
    /// attribute to assign a class as an Autumn entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EntityAttribute : Attribute
    {
        /// <summary>
        /// name of entity use for collection
        /// </summary>
        public string Name { get; set; }
    }
}