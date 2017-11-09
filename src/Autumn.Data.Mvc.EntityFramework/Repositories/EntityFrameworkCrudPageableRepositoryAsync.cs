using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Autumn.Mvc.Data.Models;
using Autumn.Mvc.Data.Models.Helpers;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Autumn.Data.Mvc.EntityFramework.Repositories
{
    public class EntityFrameworkCrudPageableRepositoryAsync<TDbContext,TEntity,TKey> : ICrudPageableRepositoryAsync<TEntity,TKey> ,IDisposable
        where TEntity :class
        where TDbContext : DbContext
    {

        private readonly TDbContext _dbContext;
        private readonly ParameterExpression _parameter;
        private readonly PropertyInfo _propertyId;

        protected TDbContext DbContext()
        {
            return _dbContext;
        }
        
        public EntityFrameworkCrudPageableRepositoryAsync(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _parameter = Expression.Parameter(typeof(TEntity));
            _propertyId = IdAttribute.GetId<TEntity>();
        }
        
        public async Task<TEntity> FindOneAsync(TKey id)
        {
            var where = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, _propertyId),
                    Expression.Constant(id)
                )
                ,_parameter
            );
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(where);
        }

        public async Task<Page<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter = null, Pageable<TEntity> pageable = null)
        {
            var query = filter ?? CommonHelper.True<TEntity>();

            var count = await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(query).CountAsync();

            var find = _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(query);
            if (pageable != null)
            {
                var offset = pageable.PageNumber * pageable.PageSize;
                var limit = pageable.PageSize;
                find = find.Skip(offset)
                    .Take(limit);
                if (pageable.Sort?.OrderBy?.Count() > 0)
                {
                    foreach (var order in pageable.Sort.OrderBy)
                    {
                        find = find.OrderBy(order);
                    }

                    foreach (var order in pageable.Sort.OrderDescendingBy)
                    {
                        find = find.OrderByDescending(order);
                    }
                }
            }

            var content = await find.ToListAsync();
            return new Page<TEntity>(content, pageable, count);
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, TKey id)
        {
            var entityDb = await FindOneAsync(id);
            if (entityDb != null)
            {
                Mapper.Map(entity, entityDb);
                _dbContext.Set<TEntity>().Update(entityDb);
                await _dbContext.SaveChangesAsync();
            }
            return entityDb;
        }

        public async Task<TEntity> DeleteAsync(TKey id)
        {
            var entity = await FindOneAsync(id);
            if (entity != null)
            {
                _dbContext.Set<TEntity>().Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
            return entity;
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}