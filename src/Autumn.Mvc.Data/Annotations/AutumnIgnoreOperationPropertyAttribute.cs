using System;

namespace Autumn.Mvc.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutumnIgnoreOperationPropertyAttribute : Attribute
    {
        public AutumnIgnoreOperationPropertyType OperationTypes { get; }

        public AutumnIgnoreOperationPropertyAttribute() : this(
            AutumnIgnoreOperationPropertyType.Insert | AutumnIgnoreOperationPropertyType.Update)
        {
        }

        public AutumnIgnoreOperationPropertyAttribute(AutumnIgnoreOperationPropertyType operationTypes)
        {
            OperationTypes = operationTypes;
        }
    }
}