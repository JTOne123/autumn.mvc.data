using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Autumn.Data.Rest.Commons;
using Autumn.Data.Rest.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Autumn.Data.Rest.Controllers
{
    public class RestRepositoryControllerAsync<T, TU, TId> : Controller
        where T : ICrudPageableRepositoryAsync<TU, TId>
        where TU : class
    {
        private readonly T _repository;

        protected T Repository()
        {
            return _repository;
        }

        public RestRepositoryControllerAsync(T repository)
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
        public virtual async Task<IActionResult> Find(Expression<Func<TU, bool>> filter, IPageable pageable)
        {
            var result = await _repository.FindAsync(filter, pageable);
            return result.TotalElements == result.NumberOfElements
                ? Ok(result)
                : StatusCode((int) HttpStatusCode.PartialContent, result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] TU entity)
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
        public virtual async Task<IActionResult> Put([FromBody] TU entity, TId id)
        {
            var result = await _repository.FindOneAsync(id);
            if (result == null) return NoContent();
            result = await _repository.UpdateAsync(entity, id);
            return Ok(result);
        }
    }
}