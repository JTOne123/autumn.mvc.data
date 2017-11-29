using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Autumn.Mvc.Data.EF.Repositories
{
    public class AutumnEntityFrameworkCrudPageableRepositoryAsync<TEntity,TKey> : AutumnCrudPageableRepositoryAsync<TEntity,TKey> 
        where TEntity :class
    {

        private readonly DbContext _dbContext;

        public AutumnEntityFrameworkCrudPageableRepositoryAsync(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task<TEntity> OnFindOneAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .SingleOrDefaultAsync(filter);
        }

        protected override async Task<AutumnPage<TEntity>> OnFindAsync(Expression<Func<TEntity, bool>> filter,
            AutumnPageable<TEntity> autumnPageable)
        {
            var count = await _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(filter).CountAsync();

            var find = _dbContext.Set<TEntity>()
                .AsNoTracking()
                .Where(filter);

            var offset = autumnPageable.PageNumber * autumnPageable.PageSize;
            var limit = autumnPageable.PageSize;
            find = find.Skip(offset)
                .Take(limit);
            if (autumnPageable.AutumnSort?.OrderBy?.Count() > 0)
            {
                find = autumnPageable.AutumnSort.OrderBy.Aggregate(find, (current, order) => current.OrderBy(order));
            }
            if (autumnPageable.AutumnSort?.OrderDescendingBy?.Count() > 0)
            {
                find = autumnPageable.AutumnSort.OrderDescendingBy.Aggregate(find,
                    (current, order) => current.OrderByDescending(order));
            }
            var content = await find.ToListAsync();
            return new AutumnPage<TEntity>(content, autumnPageable, count);
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