using System;

namespace Autumn.Mvc.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutumnKeyAttribute : Attribute
    {
        public bool Insertable { get; set; }
        public bool Updatable { get; set; }
    }
    
}