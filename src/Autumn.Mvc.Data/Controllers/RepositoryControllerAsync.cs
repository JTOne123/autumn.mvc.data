using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Autumn.Mvc.Data.Controllers
{
    [RepositoryControllerNameConvention]
    public class RepositoryControllerAsync<TEntity,TKey> : Controller, IRepositoryControllerAsync<TEntity,TKey>
        where TEntity : class 
    {
        private readonly ICrudPageableRepositoryAsync<TEntity,TKey> _repository;

        protected ICrudPageableRepositoryAsync<TEntity,TKey> Repository()
        {
            return _repository;
        }

        public RepositoryControllerAsync(ICrudPageableRepositoryAsync<TEntity,TKey> repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(TKey id)
        {
            var result = await _repository.FindOneAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
         
        [HttpGet("")]
        public virtual async Task<IActionResult> Get(Expression<Func<TEntity, bool>> filter, Pageable<TEntity> pageable)
        {
            var result = await _repository.FindAsync(filter, pageable);
            return result.TotalElements == result.NumberOfElements
                ? Ok(result)
                : StatusCode((int) HttpStatusCode.PartialContent, result);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] TEntity entity)
        {
            var result = await _repository.CreateAsync(entity);
            return Created(string.Empty, result);
        }
        
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(TKey id)
        {
            var result = await _repository.FindOneAsync(id);
            if (result == null) return NoContent();
            result = await _repository.DeleteAsync(id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put([FromBody] TEntity entity, TKey id)
        {
            var result = await _repository.FindOneAsync(id);
            if (result == null) return NoContent();
            result = await _repository.UpdateAsync(entity, id);
            return Ok(result);
        }
    }
}