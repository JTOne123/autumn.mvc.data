using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Repositories;

namespace Autumn.Mvc.Data.Dapper.Repositories
{
    public abstract class DapperCrudPageableRepositoryAsync<TEntity, TKey> : ICrudPageableRepositoryAsync<TEntity, TKey>,IDisposable
        where TEntity : class
    {
        private readonly ParameterExpression _parameter;
        private readonly PropertyInfo _propertyId;
        private readonly DbConnection _connection;
        

        public DapperCrudPageableRepositoryAsync()
        {
            _parameter = Expression.Parameter(typeof(TEntity));
            _properyId = IdAttribute.GetOrRegisterId<TEntity>();
        }
        
        
        public Task<TEntity> FindOneAsync(TKey id)
        {
            
        }

        public Task<Page<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter = null, Pageable<TEntity> pageable = null)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> CreateAsync(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> UpdateAsync(TEntity entity, TKey id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> DeleteAsync(TKey id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}