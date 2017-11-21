using System;

namespace Autumn.Mvc.Data.Annotations
{
    [Flags]
    public enum AutumnIgnoreType
    {
        Insert = 0x0,
        Update = 0x1,
        All = Insert | Update
    }
}
