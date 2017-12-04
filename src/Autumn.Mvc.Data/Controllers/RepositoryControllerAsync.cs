using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers.Exceptions;
using Autumn.Mvc.Data.Models.Exceptions;
using Autumn.Mvc.Data.Repositories;
using Autumn.Mvc.Models.Paginations;
using Microsoft.AspNetCore.Mvc;

namespace Autumn.Mvc.Data.Controllers
{
    /// <summary>
    /// api controller entity
    /// </summary>
    /// <typeparam name="TEntity">entity type</typeparam>
    /// <typeparam name="TEntityPost">entity type post operation</typeparam>
    /// <typeparam name="TEntityPut">entity type put operation</typeparam>
    /// <typeparam name="TKey">key of entity</typeparam>
    [AutumnRepositoryControllerNameConvention]
    public class RepositoryControllerAsync<TEntity,TEntityPost, TEntityPut, TKey> : Controller, IRepositoryControllerAsync<TEntity, TEntityPost, TEntityPut, TKey>
        where TEntity : class 
        where TEntityPost : class 
        where TEntityPut : class 
    {
        private readonly ICrudPageableRepositoryAsync<TEntity,TKey> _repository;
        private readonly AutumnEntityInfo _entityInfo;
        private readonly AutumnSettings _settings;

        /// <summary>
        /// associed repository of controller
        /// </summary>
        /// <returns></returns>
        protected virtual ICrudPageableRepositoryAsync<TEntity, TKey> Repository()
        {
            return _repository;
        }

        public RepositoryControllerAsync(ICrudPageableRepositoryAsync<TEntity, TKey> repository,
            AutumnSettings settings)
        {
            _repository = repository;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _entityInfo = _settings.DataSettings().EntitiesInfos[typeof(TEntity)];
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(TKey id)
        {
            try
            {
                var result = await _repository.FindOneAsync(id);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception e)
            {
                throw new GetByIdOperationException(e);
            }
        }
        
        [HttpGet("")]
        public virtual async Task<IActionResult> Get(Expression<Func<TEntity, bool>> filter, IPageable<TEntity> pageable)
        {
            try
            {
                var result = await _repository.FindAsync(filter, pageable);
                return result.TotalElements == result.NumberOfElements
                    ? Ok(result)
                    : StatusCode((int) HttpStatusCode.PartialContent, result);
            }
            catch (Exception e)
            {
                throw new AutumnGetOperationException(e);
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] [Required] TEntityPost entityPostRequest)
        {
            try
            {
                if (!ModelState.IsValid) throw new AutumnModelStateException(ModelState);
                var entity = Mapper.Map<TEntity>(entityPostRequest);
                var result = await _repository.InsertAsync(entity);
                var uri = string.Format("{0}/{1}", Request.HttpContext.Request.Path.ToString().TrimEnd('/'),
                    _entityInfo.KeyInfo.Property.GetValue(result));
                return Created(uri, result);
            }
            catch (Exception e)
            {
                throw new AutumnPostOperationException(e);
            }
        }

       [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(TKey id)
        {
            try
            {
                if (!ModelState.IsValid) throw new AutumnModelStateException(ModelState);
                var result = await _repository.FindOneAsync(id);
                if (result == null) return NoContent();
                result = await _repository.DeleteAsync(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                throw new AutumnDeleteOperationException(e);
            }
        }

      
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put([FromBody] [Required] TEntityPut entityPutRequest, TKey id)
        {
            try
            {
                if (!ModelState.IsValid) throw new AutumnModelStateException(ModelState);
                var result = await _repository.FindOneAsync(id);
                if (result == null) return NoContent();
                Mapper.Map(entityPutRequest, result);
                result = await _repository.UpdateAsync(result, id);
                return Ok(result);
            }
            catch (Exception e)
            {
                throw new AutumnPutOperationException(e);
            }
        }
    }
}