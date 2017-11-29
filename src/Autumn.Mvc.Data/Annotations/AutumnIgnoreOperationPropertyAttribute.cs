using System;

namespace Autumn.Mvc.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutumnIgnoreOperationPropertyAttribute : Attribute
    {
        /// <summary>
        /// enables or disables the property in the POST (insert) request object on the associated resource
        /// </summary>
        public bool Insertable { get; set; } = false;

        /// <summary>
        /// enables or disables the property in the POST (insert) request object on the associated resource
        /// </summary>
        public bool Updatable { get; set; } = false;
        
    }
}