using System;

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
        /// enables or disables the POST (insert) operation on the associated resource
        /// </summary>
        public bool Insertable { get; set; } = true;

        /// <summary>
        /// enables or disables the PUT (update) operation on the associated resource
        /// </summary>
        public bool Updatable { get; set; } = true;

        /// <summary>
        /// enables or disables the DELETE (delete) operation on the associated resource
        /// </summary>
        public bool Deletable { get; set; } = true;

    }
}