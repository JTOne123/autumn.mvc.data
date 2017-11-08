using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models.Paginations;
using Microsoft.AspNetCore.Mvc;

namespace Autumn.Mvc.Data.Controllers
{
    public interface IRepositoryControllerAsync<T, in TId> where T : class
    {
        Task<IActionResult> GetById(TId id);
        Task<IActionResult> Get(Expression<Func<T, bool>> filter, Pageable<T> pageable);
        Task<IActionResult> Post(T entity);
        Task<IActionResult> Delete(TId id);
        Task<IActionResult> Put(T entity, TId id);
    }
}