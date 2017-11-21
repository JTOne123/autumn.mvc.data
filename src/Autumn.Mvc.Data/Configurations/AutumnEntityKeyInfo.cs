using System.Reflection;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnEntityKeyInfo
    {
        public PropertyInfo Property { get; }

        public AutumnEntityKeyInfo(PropertyInfo property, AutumnKeyAttribute keyAttribute)
        {
            Property = property;
        }
    }
}