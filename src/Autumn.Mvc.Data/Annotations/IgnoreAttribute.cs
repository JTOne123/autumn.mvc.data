using System;

namespace Autumn.Mvc.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        /// <summary>
        /// enables or disables the property in the POST (insert) request object on the associated resource
        /// </summary>
        public bool Insertable { get; set; } 

        /// <summary>
        /// enables or disables the property in the POST (insert) request object on the associated resource
        /// </summary>
        public bool Updatable { get; set; } 
    }
}