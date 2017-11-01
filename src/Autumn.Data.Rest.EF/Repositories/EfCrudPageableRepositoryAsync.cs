using System;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Data.Rest.Commons;
using Autumn.Data.Rest.Repositories;
using Autumn.Data.Rest.Rsql;
using Microsoft.EntityFrameworkCore;

namespace Autumn.Data.Rest.EF.Repositories
{
    public class EfCrudPageableRepositoryAsync<D,T,TId> : ICrudPageableRepositoryAsync<T,TId> 
        where T : class
        where D : DbContext
    {

        private readonly D _dbContext;

        protected D DbContext()
        {
            return _dbContext;
        }

        protected EfCrudPageableRepositoryAsync(D dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<T> FindOneAsync(TId id)
        {
            return await _dbContext.FindAsync<T>(new object[] {id});
        }

        public Task<IPage<T>> FindAsync(Expression<Func<T, bool>> filter = null, IPageable pageable = null)
        {
            var query = _dbContext.Set<T>()
                .Where(filter ?? RsqlHelper.True<T>());
            
            if (pageable != null)
            {
                query.Skip(pageable.PageSize)
            }                
                
            throw new NotImplementedException();
        }

        public Task<T> CreateAsync(T entity)
        {
            
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync(T entity, TId id)
        {
            throw new NotImplementedException();
        }

        public Task<T> DeleteAsync(TId id)
        {
            throw new NotImplementedException();
        }
    }
}