using System;

namespace Autumn.Mvc.Data.Annotations
{
    [Flags]
    public enum AutumnIgnoreType
    {
        Post = 0x0,
        Put = 0x1,
    }
}
