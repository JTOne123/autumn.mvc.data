using System;

namespace Autumn.Mvc.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ResourceAttribute : Attribute
    {
        /// <summary>
        /// name of resource
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