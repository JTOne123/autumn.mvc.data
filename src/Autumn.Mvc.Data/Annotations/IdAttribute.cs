using System;

namespace Autumn.Mvc.Data.Annotations
{
    /// <summary>
    /// attribute to identify the property of the entity as key uniqueness
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IdAttribute : Attribute
    {
    }
}