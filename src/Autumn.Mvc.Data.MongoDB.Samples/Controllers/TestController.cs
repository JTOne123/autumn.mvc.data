using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.MongoDB.Samples.Models;
using Autumn.Mvc.Data.Repositories;
using Autumn.Mvc.Models.Paginations;
using Microsoft.AspNetCore.Mvc;

namespace Autumn.Mvc.Data.MongoDB.Samples.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {

        private ICrudPageableRepositoryAsync<CustomerV4, string> _repositoryAsync;

        public TestController(ICrudPageableRepositoryAsync<CustomerV4, string> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Expression<Func<CustomerV4,bool>> filter, IPageable<CustomerV4> pageable)
        {
            var result = await _repositoryAsync.FindAsync(filter,pageable);
            return Ok(result);
        }
    }
}