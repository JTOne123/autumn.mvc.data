using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Autumn.Data.Mvc.EntityFramework.Repositories
{
    public class EntityFrameworkCrudPageableRepositoryAsync<TEntity,TKey> : CrudPageableRepositoryAsync<TEntity,TKey> 
        where TEntity :class
    {

        private readonly DbContext _dbContext;

        public EntityFrameworkCrudPageableRepositoryAsync(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task<TEntity> OnFindOneAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(filter);
        }

        protected override async Task<Page<TEntity>> OnFindAsync(Expression<Func<TEntity, bool>> filter,
            Pageable<TEntity> pageable)
        {
            var count = await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(filter).CountAsync();

            var find = _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(filter);

            var offset = pageable.PageNumber * pageable.PageSize;
            var limit = pageable.PageSize;
            find = find.Skip(offset)
                .Take(limit);
            if (pageable.Sort?.OrderBy?.Count() > 0)
            {
                find = pageable.Sort.OrderBy.Aggregate(find, (current, order) => current.OrderBy(order));
            }
            if (pageable.Sort?.OrderDescendingBy?.Count() > 0)
            {
                find = pageable.Sort.OrderDescendingBy.Aggregate(find,
                    (current, order) => current.OrderByDescending(order));
            }
            var content = await find.ToListAsync();
            return new Page<TEntity>(content, pageable, count);
        }

        protected override async Task<TEntity> OnInsertAsync(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        protected override async Task<TEntity> OnUpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            _dbContext.Set<TEntity>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        protected override async Task<TEntity> OnDeleteAsync(Expression<Func<TEntity, bool>> filter)
        {
            var entityDb = await OnFindOneAsync(filter);
            if (entityDb == null) return null;
            _dbContext.Set<TEntity>().Remove(entityDb);
            await _dbContext.SaveChangesAsync();
            return entityDb;
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}