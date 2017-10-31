using System;
using System.Linq.Expressions;
using Autumn.Data.Rest.Commons;

namespace Autumn.Data.Rest.Repositories
{
    /// <summary>
    /// intefaceof CRUD repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface ICrudPageableRepository<T, in TId>
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
        IPage<T> Find(Expression<Func<T, bool>> filter=null, IPageable pageable=null);

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
        /// <returns></returns>
        T Update(T entity);

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