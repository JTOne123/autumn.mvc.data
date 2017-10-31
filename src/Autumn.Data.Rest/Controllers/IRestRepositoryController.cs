using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Data.Rest.Commons;
using Microsoft.AspNetCore.Mvc;

namespace Autumn.Data.Rest.Controllers
{
    public interface IRestRepositoryControllerAsync<TU, TId> where TU : class
    {
        Task<IActionResult> FindById(TId id);
        Task<IActionResult> Find(Expression<Func<TU, bool>> filter, IPageable pageable);
    }
}