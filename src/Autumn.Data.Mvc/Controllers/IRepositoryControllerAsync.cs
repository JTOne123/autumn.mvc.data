using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Data.Mvc.Models.Paginations;
using Microsoft.AspNetCore.Mvc;

namespace Autumn.Data.Mvc.Controllers
{
    public interface IRepositoryControllerAsync<T, in TId> where T : class
    {
        Task<IActionResult> FindById(TId id);
        Task<IActionResult> Find(Expression<Func<T, bool>> filter, Pageable<T> pageable);
        Task<IActionResult> Post(T entity);
        Task<IActionResult> Delete(TId id);
        Task<IActionResult> Put(T entity, TId id);
    }
}