using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Data.Rest.Commons;

namespace Autumn.Data.Rest.Repositories
{
    /// <summary>
    /// intefaceof CRUD repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public interface ICrudPageableRepositoryAsync<T, in TId>
    {
        /// <summary>
        /// find enity by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> FindOneAsync(TId id);

        /// <summary>
        /// find entity by criteria
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pageable"></param>
        /// <returns></returns>
        Task<IPage<T>> FindAsync(Expression<Func<T, bool>> filter=null, IPageable pageable=null);

        /// <summary>
        /// create entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// update entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// update entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> UpdateAsync(T entity, TId id);

        /// <summary>
        /// delete entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> DeleteAsync(TId id);
    }
}