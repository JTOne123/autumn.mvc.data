using System.Collections.Generic;

namespace Autumn.Data.Rest.Commons
{
    /// <summary>
    /// interface of page 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPage<T>
    {
        /// <summary>
        /// content of page
        /// </summary>
        List<T> Content { get; }
        /// <summary>
        /// total elements 
        /// </summary>
        long TotalElements { get; }
        /// <summary>
        /// number of page start to zero
        /// </summary>
        int Number { get; }
        /// <summary>
        /// number of elements of page
        /// </summary>
        int NumberOfElements { get; }
        /// <summary>
        /// total pages
        /// </summary>
        int TotalPages { get; }
        /// <summary>
        /// has content
        /// </summary>
        bool HasContent { get; }
        /// <summary>
        /// has next page
        /// </summary>
        bool HasNext { get; }
        /// <summary>
        /// has previous page
        /// </summary>
        bool HasPrevious { get; }
        /// <summary>
        /// is first page
        /// </summary>
        bool IsFirst { get; }
        /// <summary>
        /// is last page
        /// </summary>
        bool IsLast { get; }
    }
}