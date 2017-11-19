using System.Reflection;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnEntityKeyInfo
    {
        public bool Insertable { get;  }
        public bool Updatable { get; }
        public PropertyInfo Property { get; }
        
        public AutumnEntityKeyInfo(){}

        public AutumnEntityKeyInfo(PropertyInfo property, AutumnKeyAttribute keyAttribute)
        {
            Property = property;
            Insertable = keyAttribute.Insertable;
            Updatable = keyAttribute.Updatable;
        }
    }
}