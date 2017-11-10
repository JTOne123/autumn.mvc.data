using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Controllers
{
    [RepositoryControllerNameConvention]
    public class RepositoryControllerAsync<TEntity,TKey> : Controller, IRepositoryControllerAsync<TEntity,TKey>
        where TEntity : class 
    {
        private readonly ICrudPageableRepositoryAsync<TEntity,TKey> _repository;
        private readonly PropertyInfo _idProperty;
        private readonly bool _idInsertable;
        private readonly bool _idUpdatable;

        protected ICrudPageableRepositoryAsync<TEntity, TKey> Repository()
        {
            return _repository;
        }

        public RepositoryControllerAsync(ICrudPageableRepositoryAsync<TEntity,TKey> repository)
        {
            _repository = repository;
            _idProperty = IdAttribute.GetOrRegisterId<TEntity>();
            var attribute = _idProperty.GetCustomAttribute<IdAttribute>();
            _idInsertable = attribute.Insertable;
            _idUpdatable = attribute.Updatable;
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
        public virtual async Task<IActionResult> Post([FromBody] [Required] TEntity entity)
        {
            if (ModelState.IsValid)
            {
                if (!_idInsertable) _idProperty.SetValue(entity, null);
                var result = await _repository.CreateAsync(entity);
                var uri = string.Format("{0}/{1}", Request.HttpContext.Request.Path.ToString().TrimEnd('/'),
                    _idProperty.GetValue(result));
                return Created(uri, result);
            }
            else
            {
                throw BuildException(ModelState);
            }
        }


        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(TKey id)
        {
            if (ModelState.IsValid)
            {
                var result = await _repository.FindOneAsync(id);
                if (result == null) return NoContent();
                result = await _repository.DeleteAsync(id);
                return Ok(result);
            }
            else
            {
                throw BuildException(ModelState);
            }
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put([FromBody][Required] TEntity entity,TKey id)
        {
            if (ModelState.IsValid)
            {
                if (!_idUpdatable) _idProperty.SetValue(entity, id);
                var result = await _repository.FindOneAsync(id);
                if (result == null) return NoContent();
                result = await _repository.UpdateAsync(entity, id);
                return Ok(result);
            }
            else
            {
                throw BuildException(ModelState);
            }
        }


        private static Exception BuildException(ModelStateDictionary stateDictionary)
        {
            var stringbuilder = new StringBuilder();
            foreach (var item in stateDictionary.Values)
            foreach (var error in item.Errors)
                stringbuilder.Append(error.ErrorMessage);
            return new Exception(stringbuilder.ToString());
        }
    }
}