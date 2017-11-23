using System;

namespace Autumn.Mvc.Data.Annotations
{
    [Flags]
    public enum AutumnIgnoreOperationType
    {
        Insert = 1,
        Update = 10,
        Delete = 100
    }
    
  
}