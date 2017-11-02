namespace Autumn.Data.Rest.Paginations
{
    public class Pageable<T> where T :class
    {
        public int PageNumber { get; }

        public int PageSize { get; }

        public Sort<T> Sort { get; }

        public Pageable(int pageNumber, int pageSize, Sort<T> sort = null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Sort = sort;
        }
    }
}