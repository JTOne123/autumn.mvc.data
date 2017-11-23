using System;

namespace Autumn.Mvc.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutumnIgnoreOperationAttribute : Attribute
    {
        public AutumnIgnoreOperationType OperationTypes { get; }

        public AutumnIgnoreOperationAttribute() : this(AutumnIgnoreOperationType.Insert |
                                                       AutumnIgnoreOperationType.Update |
                                                       AutumnIgnoreOperationType.Delete)
        {
        }

        public AutumnIgnoreOperationAttribute(AutumnIgnoreOperationType operationTypes)
        {
            OperationTypes = operationTypes;
        }
    }
}