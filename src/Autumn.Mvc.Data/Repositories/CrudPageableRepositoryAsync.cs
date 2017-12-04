using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Models.Paginations;
using Autumn.Mvc.Models.Queries;

namespace Autumn.Mvc.Data.Repositories
{
    public abstract class CrudPageableRepositoryAsync<TEntity,TKey> : ICrudPageableRepositoryAsync<TEntity,TKey>
        where TEntity : class
    {

        private readonly ParameterExpression _parameter;
        private readonly AutumnSettings _settings;
        private readonly AutumnEntityInfo _entityInfo;

        protected CrudPageableRepositoryAsync(AutumnSettings settings)
        {
            _parameter = Expression.Parameter(typeof(TEntity));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _entityInfo = _settings.DataSettings().EntitiesInfos[typeof(TEntity)];
        }

        #region FindOneAsync

        public async Task<TEntity> FindOneAsync(TKey id)
        {
            var filter = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, _entityInfo.KeyInfo.Property),
                    Expression.Constant(id)
                )
                , _parameter
            );
            var result = await OnFindOneAsync(filter);
            
            return result;
        }

        protected abstract Task<TEntity> OnFindOneAsync(Expression<Func<TEntity,bool>> filter);
        
        #endregion
        
        #region FindAsync

        public async Task<Page<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter = null,
            IPageable<TEntity> pageable = null)
        {
            return await OnFindAsync(
                filter ?? QueryExpressionHelper.True<TEntity>(),
                pageable ?? new Pageable<TEntity>(0, _settings.PageSize)
            );
        }
       
        protected abstract Task<Page<TEntity>> OnFindAsync(Expression<Func<TEntity,bool>> filter,IPageable<TEntity> pageable); 
        
        #endregion

        #region InsertAsync
        
        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            return await OnInsertAsync(entity);
        }

        protected abstract Task<TEntity> OnInsertAsync(TEntity entity);
        
        #endregion
        
        #region UpdateAsync

        public async Task<TEntity> UpdateAsync(TEntity entity, TKey id)
        {
            var filter = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, _entityInfo.KeyInfo.Property),
                    Expression.Constant(id)
                ),
                _parameter
            );
            return await OnUpdateAsync(entity, filter);
        }

        protected abstract Task<TEntity> OnUpdateAsync(TEntity entity,Expression<Func<TEntity, bool>> filter);
        
        #endregion
        
        #region DeleteAsync

        public async Task<TEntity> DeleteAsync(TKey id)
        {
            var filter = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, _entityInfo.KeyInfo.Property),
                    Expression.Constant(id)
                )
                ,
                _parameter
            );
            return await OnDeleteAsync(filter);
        }


        protected abstract Task<TEntity> OnDeleteAsync(Expression<Func<TEntity, bool>> filter);
        
        #endregion
        
    }
}