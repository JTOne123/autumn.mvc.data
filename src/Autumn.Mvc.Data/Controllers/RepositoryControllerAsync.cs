using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Autumn.Mvc.Data.Controllers
{
    [RepositoryControllerNameConvention]
    public class RepositoryControllerAsync<T,TId> : Controller, IRepositoryControllerAsync<T,TId>
        where T : class 
    {
        private readonly ICrudPageableRepositoryAsync<T,TId> _repository;

        protected ICrudPageableRepositoryAsync<T,TId> Repository()
        {
            return _repository;
        }

        public RepositoryControllerAsync(ICrudPageableRepositoryAsync<T,TId> repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> FindById(TId id)
        {
            var result = await _repository.FindOneAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

           
        [HttpGet("")]
        public virtual async Task<IActionResult> Find(Expression<Func<T, bool>> filter, Pageable<T> pageable)
        {
            var result = await _repository.FindAsync(filter, pageable);
            return result.TotalElements == result.NumberOfElements
                ? Ok(result)
                : StatusCode((int) HttpStatusCode.PartialContent, result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] T entity)
        {
            var result = await _repository.CreateAsync(entity);
            return Created(string.Empty, result);
        }
        
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(TId id)
        {
            var result = await _repository.FindOneAsync(id);
            if (result == null) return NoContent();
            result = await _repository.DeleteAsync(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put([FromBody] T entity, TId id)
        {
            var result = await _repository.FindOneAsync(id);
            if (result == null) return NoContent();
            result = await _repository.UpdateAsync(entity, id);
            return Ok(result);
        }
    }
}