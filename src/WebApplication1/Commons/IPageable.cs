using System.Collections.Generic;
using Newtonsoft.Json;
using WebApplication1.Repositories;

namespace WebApplication1.Commons
{
    public interface IPageable
    {
        int PageNumber { get; }
        int PageSize { get; }
        int Offset { get; }
        List<Sort> Sort { get; }
    }
}