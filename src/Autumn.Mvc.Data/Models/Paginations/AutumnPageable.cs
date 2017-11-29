namespace Autumn.Mvc.Data.Models.Paginations
{
    /// <summary>
    /// paging request object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AutumnPageable<T> where T :class
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
        public AutumnSort<T> AutumnSort { get; }

        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="autumnSort"></param>
        public AutumnPageable(int pageNumber, int pageSize, AutumnSort<T> autumnSort = null)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            AutumnSort = autumnSort;
        }
    }
}