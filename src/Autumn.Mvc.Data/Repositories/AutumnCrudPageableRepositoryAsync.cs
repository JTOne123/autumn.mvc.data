using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Models.Queries;

namespace Autumn.Mvc.Data.Repositories
{
    public abstract class AutumnCrudPageableRepositoryAsync<TEntity,TKey> : IAutumnCrudPageableRepositoryAsync<TEntity,TKey>
        where TEntity : class
    {

        private readonly ParameterExpression _parameter;
         protected AutumnEntityInfo EntityInfo { get; }

        protected AutumnCrudPageableRepositoryAsync()
        {
            EntityInfo = AutumnApplication.Current.EntitiesInfos[typeof(TEntity)];
            _parameter = Expression.Parameter(typeof(TEntity));
        }

        #region FindOneAsync

        public async Task<TEntity> FindOneAsync(TKey id)
        {
            var filter = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, EntityInfo.KeyInfo.Property),
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

        public async Task<AutumnPage<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter = null,
            AutumnPageable<TEntity> autumnPageable = null)
        {
            return await OnFindAsync(
                filter ?? AutumnQueryHelper.True<TEntity>(),
                autumnPageable ?? new AutumnPageable<TEntity>(0, AutumnApplication.Current.DefaultPageSize)
            );
        }

        protected abstract Task<AutumnPage<TEntity>> OnFindAsync(Expression<Func<TEntity,bool>> filter,AutumnPageable<TEntity> autumnPageable); 
        
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
                    Expression.Property(_parameter, EntityInfo.KeyInfo.Property),
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
                    Expression.Property(_parameter, EntityInfo.KeyInfo.Property),
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