using System;

namespace Autumn.Mvc.Data.Annotations
{
    [Flags]
    public enum AutumnIgnoreOperationPropertyType
    {
        Insert = 1,
        Update = 10,
    }
}
