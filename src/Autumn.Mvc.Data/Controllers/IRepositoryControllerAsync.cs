using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Controllers.Exceptions;
using Autumn.Mvc.Models.Paginations;
using Microsoft.AspNetCore.Mvc;

namespace Autumn.Mvc.Data.Controllers
{
    /// <summary>
    /// interface of repository controller 
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="TEntityPost">entity type post operation</typeparam>
    /// <typeparam name="TEntityPut">entity type put operation</typeparam>
    /// <typeparam name="TKey">key of entity</typeparam>
    public interface IRepositoryControllerAsync<TEntity, in TEntityPost, in TEntityPut, in TKey> 
        where TEntity : class
        where TEntityPost : class
        where TEntityPut : class
    {
        
        /// <summary>
        /// get entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="GetByIdOperationException"></exception>
        Task<IActionResult> GetById(TKey id);
        
        /// <summary>
        /// get entities by search criteria
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="autumnPageable"></param>
        /// <returns></returns>
        /// <exception cref="AutumnGetOperationException"></exception>
        Task<IActionResult> Get(Expression<Func<TEntity, bool>> filter, IPageable<TEntity> autumnPageable);
        
        /// <summary>
        /// add new entity in repository
        /// </summary>
        /// <param name="entityPostRequest">entity to add</param>
        /// <returns></returns>
        /// <exception cref="AutumnPostOperationException"></exception>
        Task<IActionResult> Post(TEntityPost entityPostRequest);
        
        /// <summary>
        /// delete entity of repository
        /// </summary>
        /// <param name="id">id of entity to delete</param>
        /// <returns></returns>
        /// <exception cref="AutumnDeleteOperationException"></exception>
        Task<IActionResult> Delete(TKey id);
        
        /// <summary>
        /// update entity in repository
        /// </summary>
        /// <param name="entityPutRequest">new entity value</param>
        /// <param name="id">id of entity to update</param>
        /// <returns></returns>
        /// <exception cref="AutumnPutOperationException"></exception>
        Task<IActionResult> Put(TEntityPut entityPutRequest, TKey id);
    }
}