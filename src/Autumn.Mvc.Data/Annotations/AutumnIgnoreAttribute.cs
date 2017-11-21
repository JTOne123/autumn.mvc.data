using System;

namespace Autumn.Mvc.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutumnIgnoreAttribute : Attribute
    {
        public AutumnIgnoreType Type { get; }

        public AutumnIgnoreAttribute() : this(AutumnIgnoreType.Post | AutumnIgnoreType.Put)
        {
        }

        public AutumnIgnoreAttribute(AutumnIgnoreType type)
        {
            Type = type;
        }
    }
}