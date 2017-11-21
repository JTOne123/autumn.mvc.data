using System;
using System.Text.RegularExpressions;

namespace Autumn.Mvc.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AutumnEntityAttribute : Attribute
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}