using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models.Paginations;
using Microsoft.AspNetCore.Mvc;

namespace Autumn.Mvc.Data.Controllers
{
    public interface IRepositoryControllerAsync<TEntity, in TKey> where TEntity : class
    {
        Task<IActionResult> GetById(TKey id);
        Task<IActionResult> Get(Expression<Func<TEntity, bool>> filter, Pageable<TEntity> pageable);
        Task<IActionResult> Post(TEntity entity);
        Task<IActionResult> Delete(TKey id);
        Task<IActionResult> Put(TEntity entity, TKey id);
    }
}