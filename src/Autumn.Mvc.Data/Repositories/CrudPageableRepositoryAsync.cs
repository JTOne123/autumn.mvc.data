using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Helpers;
using Autumn.Mvc.Data.Models.Paginations;

namespace Autumn.Mvc.Data.Repositories
{
    public abstract class CrudPageableRepositoryAsync<TEntity,TKey> : ICrudPageableRepositoryAsync<TEntity,TKey>
        where TEntity : class
    {

        private readonly ParameterExpression _parameter;
        protected AutumnEntityInfo EntityInfo { get; }

        protected CrudPageableRepositoryAsync()
        {
            EntityInfo = AutumnSettings.Instance.EntitiesInfos[typeof(TEntity)];
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
            return await OnFindOneAsync(filter);
        }

        protected abstract Task<TEntity> OnFindOneAsync(Expression<Func<TEntity,bool>> filter);
        
        #endregion
        
        #region FindAsync

        public async Task<Page<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter = null,
            Pageable<TEntity> pageable = null)
        {
            return await OnFindAsync(
                filter ?? CommonHelper.True<TEntity>(),
                pageable ?? new Pageable<TEntity>(0, AutumnSettings.Instance.DefaultPageSize)
            );
        }

        protected abstract Task<Page<TEntity>> OnFindAsync(Expression<Func<TEntity,bool>> filter,Pageable<TEntity> pageable); 
        
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