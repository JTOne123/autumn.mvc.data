using System;
using System.Text.RegularExpressions;

namespace Autumn.Mvc.Data.Annotations
{
    /// <summary>
    /// attribute to assign a class as an Autumn entity
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AutumnEntityAttribute : Attribute
    {
        /// <summary>
        /// name of entity, used by the API to define the path to the resource {version}/{name}
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// version of entity,  used by the API to define the path to the resource {version}/{name}
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// to exclude REST operations on the associated resource
        /// </summary>
        public AutumnIgnoreOperationType? IgnoreOperations { get; set; }
    }
}