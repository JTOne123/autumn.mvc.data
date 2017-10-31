using System.Collections.Generic;

namespace Autumn.Data.Rest.Commons
{
    public class Pageable : IPageable
    {
        public int PageNumber { get; }

        public int PageSize { get; }

        public List<Sort> Sort { get; }

        public Pageable(int pageNumber, int pageSize, params Sort[] sorts)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
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