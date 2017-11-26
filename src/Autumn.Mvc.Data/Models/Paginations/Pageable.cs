namespace Autumn.Mvc.Data.Models.Paginations
{
    /// <summary>
    /// paging request object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pageable<T> where T :class
    {
        /// <summary>
        /// page number 
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// page size
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// sort expression
        /// </summary>
        public Sort<T> Sort { get; }

        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="sort"></param>
        public Pageable(int pageNumber, int pageSize, Sort<T> sort = null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Sort = sort;
        }
    }
}