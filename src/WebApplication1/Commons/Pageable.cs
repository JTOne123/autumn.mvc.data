using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApplication1.Repositories;

namespace WebApplication1.Commons
{
    public class Pageable : IPageable
    {
        public int PageNumber { get; }

        public int PageSize { get; }

        public int Offset { get; }

        public List<Sort> Sort { get; }

        public Pageable(int pageNumber, int pageSize, params Sort[] sorts)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Offset = PageNumber * PageSize;
            if (sorts != null && sorts.Length > 0)
            {
                Sort = new List<Sort>(sorts);
            }
            else
            {
                Sort = new List<Sort>();
            }
        }
    }
}