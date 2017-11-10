using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models.Paginations;

namespace Autumn.Mvc.Data.Repositories
{
    /// <summary>
    /// intefaceof CRUD repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface ICrudPageableRepository<T, in TId>
        where T : class
    {
        /// <summary>
        /// find enity by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T FindOne(TId id);

        /// <summary>
        /// find entity by criteria
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pageable"></param>
        /// <returns></returns>
        Page<T> Find(Expression<Func<T, bool>> filter=null, Pageable<T> pageable=null);

        /// <summary>
        /// create entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Create(T entity);

        /// <summary>
        /// update entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        T Update(T entity, TId id);

        /// <summary>
        /// delete entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Delete(TId id);
    }
}