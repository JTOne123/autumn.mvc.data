using System.Reflection;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.Configurations
{
    public class EntityKeyInfo
    {
        public PropertyInfo Property { get; }

        public EntityKeyInfo(PropertyInfo property, KeyAttribute keyAttribute)
        {
            Property = property;
        }
    }
}