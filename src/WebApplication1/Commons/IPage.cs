using System.Collections.Generic;

namespace WebApplication1.Commons
{
    public interface IPage<T>
    {
        List<T> Content { get; }
        long TotalElements { get; }
        int Number { get; }
        int NumberOfElements { get; }
        int TotalPages { get; }
        bool HasContent { get; }
        bool HasNext { get; }
        bool HasPrevious { get; }
        bool IsFirst { get; }
        bool IsLast { get; }
    }
}