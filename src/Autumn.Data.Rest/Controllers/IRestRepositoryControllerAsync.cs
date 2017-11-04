using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Data.Rest.Paginations;
using Microsoft.AspNetCore.Mvc;

namespace Autumn.Data.Rest.Controllers
{
    public interface IRestRepositoryControllerAsync<T, TId> where T : class
    {
        Task<IActionResult> FindById(TId id);
        Task<IActionResult> Find(Expression<Func<T, bool>> filter, Pageable<T> pageable);
        Task<IActionResult> Post(T entity);
        Task<IActionResult> Delete(TId id);
        Task<IActionResult> Put(T entity, TId id);
    }
}