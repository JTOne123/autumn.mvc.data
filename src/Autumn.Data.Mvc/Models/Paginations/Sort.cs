using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Autumn.Data.Mvc.Models.Paginations
{
    /// <summary>
    /// sort expressions 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Sort<T> where T : class
    {
        public IEnumerable<Expression<Func<T, object>>> OrderBy { get; }
        public IEnumerable<Expression<Func<T,object>>> OrderDescendingBy { get; }

        public Sort(IEnumerable<Expression<Func<T, object>>> orderBy = null, IEnumerable<Expression<Func<T, object>>> orderDescendingBy = null)
        {
            OrderBy = orderBy;
            OrderDescendingBy = orderDescendingBy;
        }
    }
}