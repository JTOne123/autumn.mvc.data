using System;

namespace Autumn.Mvc.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutumnPropertyAttribute : Attribute
    {
        public bool Insertable { get; set; }
        public bool Updatable { get; set; }

        public AutumnPropertyAttribute()
        {
            Insertable = true;
            Updatable = true;
        }
    }
}