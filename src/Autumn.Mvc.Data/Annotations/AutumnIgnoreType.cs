using System;

namespace Autumn.Mvc.Data.Annotations
{
    [Flags]
    public enum AutumnIgnoreType
    {
        Post = 0x1,
        Put = 0x2,
        All = Post | Put
    }
}
