using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.MongoDB.Configurations;
using Autumn.Mvc.Data.Repositories;
using MongoDB.Driver;

namespace Autumn.Mvc.Data.MongoDB.Repositories
{
    public class MongoCrudPageableRepositoryAsync<TEntity,TKey> : CrudPageableRepositoryAsync<TEntity,TKey> 
        where TEntity :class
    {
        private readonly IMongoCollection<TEntity> _collection;
        private readonly FilterDefinitionBuilder<TEntity> _filterDefinitionBuilder;

        /// <summary>
        /// initialise une new instance of class
        /// </summary>
        /// <param name="settings"></param>
        public MongoCrudPageableRepositoryAsync(AutumnMongoSettings settings)
        {
            _filterDefinitionBuilder = new FilterDefinitionBuilder<TEntity>();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            var collectionName = EntityInfo.Name;
            _collection = database.GetCollection<TEntity>(collectionName);
        }

        #region FindOneAsync
        
        protected override async Task<TEntity> OnFindOneAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await _collection
                            .Find(filter)
                            .SingleOrDefaultAsync();
        }
        
        #endregion
        
        #region FindAsync

        protected override async Task<Page<TEntity>> OnFindAsync(Expression<Func<TEntity, bool>> filter,
            Pageable<TEntity> pageable)
        {

            var count = (int) await _collection.CountAsync(filter);
            var find = _collection.Find(filter);

            var offset = pageable.PageNumber * pageable.PageSize;
            var limit = pageable.PageSize;
            find = find.Skip(offset).Limit(limit);
            if (pageable.Sort?.OrderBy?.Count() > 0)
            {
                var isFirst = true;
                foreach (var item in pageable.Sort.OrderBy)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        find = find.SortBy(item);
                    }
                    else
                    {
                        find = ((IOrderedFindFluent<TEntity, TEntity>) find).ThenBy(item);
                    }
                }
            }
            if (pageable.Sort?.OrderDescendingBy?.Count() > 0)
            {
                var isFirst = true;
                foreach (var item in pageable.Sort.OrderDescendingBy)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        find = find.SortByDescending(item);
                    }
                    else
                    {
                        find = ((IOrderedFindFluent<TEntity, TEntity>) find).ThenByDescending(item);
                    }
                }
            }
            var content = await find.ToListAsync();
            return new Page<TEntity>(content, pageable, count);
        }

        #endregion
        
        #region InsertAsync

        protected override async Task<TEntity> OnInsertAsync(TEntity entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }
        
        #endregion

        #region UpdateAsync
        
        protected override async Task<TEntity> OnUpdateAsync(TEntity entity, Expression<Func<TEntity, bool>> filter)
        {
            var filterDefinition = _filterDefinitionBuilder.Where(filter);
            await _collection.ReplaceOneAsync(filterDefinition, entity);
            return entity;
        }
        
        #endregion
        
        #region DeleteAsync

        protected override async Task<TEntity> OnDeleteAsync(Expression<Func<TEntity, bool>> filter)
        {

            var filterDefinition = _filterDefinitionBuilder.Where(filter);
            return await _collection.FindOneAndDeleteAsync(filterDefinition);
        }

        #endregion
    }
}