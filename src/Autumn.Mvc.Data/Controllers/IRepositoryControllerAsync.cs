using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models.Paginations;
using Microsoft.AspNetCore.Mvc;

namespace Autumn.Mvc.Data.Controllers
{
    public interface IRepositoryControllerAsync<TEntity, in TEntityPost, in TEntityPut, in TKey> 
        where TEntity : class
        where TEntityPost : class
        where TEntityPut : class
    {
        Task<IActionResult> GetById(TKey id);
        Task<IActionResult> Get(Expression<Func<TEntity, bool>> filter, Pageable<TEntity> pageable);
        Task<IActionResult> Post(TEntityPost entity);
        Task<IActionResult> Delete(TKey id);
        Task<IActionResult> Put(TEntityPut entity, TKey id);
    }
}