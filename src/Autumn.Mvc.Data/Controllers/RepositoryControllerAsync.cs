using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers.Exceptions;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Repositories;
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
    [RepositoryControllerNameConvention]
    public class RepositoryControllerAsync<TEntity,TEntityPost, TEntityPut, TKey> : Controller, IRepositoryControllerAsync<TEntity, TEntityPost, TEntityPut, TKey>
        where TEntity : class 
        where TEntityPost : class 
        where TEntityPut : class 
    {
        private readonly ICrudPageableRepositoryAsync<TEntity,TKey> _repository;
        private readonly AutumnEntityInfo _entityInfo;

        /// <summary>
        /// associed repository of controller
        /// </summary>
        /// <returns></returns>
        protected virtual ICrudPageableRepositoryAsync<TEntity, TKey> Repository()
        {
            return _repository;
        }

        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="repository"></param>
        public RepositoryControllerAsync(ICrudPageableRepositoryAsync<TEntity, TKey> repository)
        {
            _repository = repository;
            _entityInfo = AutumnApplication.Current.EntitiesInfos[typeof(TEntity)];
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
        public virtual async Task<IActionResult> Get(Expression<Func<TEntity, bool>> filter, Pageable<TEntity> pageable)
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
                throw new GetOperationException(e);
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Post([FromBody] [Required] TEntityPost entityPostRequest)
        {
            try
            {
                if (!ModelState.IsValid) throw new ModelStateException(ModelState);
                var entity = Mapper.Map<TEntity>(entityPostRequest);
                var result = await _repository.InsertAsync(entity);
                var uri = string.Format("{0}/{1}", Request.HttpContext.Request.Path.ToString().TrimEnd('/'),
                    _entityInfo.KeyInfo.Property.GetValue(result));
                return Created(uri, result);
            }
            catch (Exception e)
            {
                throw new PostOperationException(e);
            }
        }

       [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(TKey id)
        {
            try
            {
                if (!ModelState.IsValid) throw new ModelStateException(ModelState);
                var result = await _repository.FindOneAsync(id);
                if (result == null) return NoContent();
                result = await _repository.DeleteAsync(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                throw new DeleteOperationException(e);
            }
        }

      
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put([FromBody] [Required] TEntityPut entityPutRequest, TKey id)
        {
            try
            {
                if (!ModelState.IsValid) throw new ModelStateException(ModelState);
                var result = await _repository.FindOneAsync(id);
                if (result == null) return NoContent();
                Mapper.Map(entityPutRequest, result);
                result = await _repository.UpdateAsync(result, id);
                return Ok(result);
            }
            catch (Exception e)
            {
                throw new PutOperationException(e);
            }
        }
    }
}