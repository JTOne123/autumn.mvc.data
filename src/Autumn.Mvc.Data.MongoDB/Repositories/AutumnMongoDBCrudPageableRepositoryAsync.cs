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
    public class AutumnMongoDBCrudPageableRepositoryAsync<TEntity,TKey> : AutumnCrudPageableRepositoryAsync<TEntity,TKey> 
        where TEntity :class
    {
        private readonly IMongoCollection<TEntity> _collection;
        private readonly FilterDefinitionBuilder<TEntity> _filterDefinitionBuilder;

        /// <summary>
        /// initialise une new instance of class
        /// </summary>
        /// <param name="settings"></param>
        public AutumnMongoDBCrudPageableRepositoryAsync(AutumnMongoSettings settings)
        {
            _filterDefinitionBuilder = new FilterDefinitionBuilder<TEntity>();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.Database);
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

        protected override async Task<AutumnPage<TEntity>> OnFindAsync(Expression<Func<TEntity, bool>> filter,
            AutumnPageable<TEntity> autumnPageable)
        {

            var count = (int) await _collection.CountAsync(filter);
            var find = _collection.Find(filter);

            var offset = autumnPageable.PageNumber * autumnPageable.PageSize;
            var limit = autumnPageable.PageSize;
            find = find.Skip(offset).Limit(limit);
            if (autumnPageable.AutumnSort?.OrderBy?.Count() > 0)
            {
                var isFirst = true;
                foreach (var item in autumnPageable.AutumnSort.OrderBy)
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
            if (autumnPageable.AutumnSort?.OrderDescendingBy?.Count() > 0)
            {
                var isFirst = true;
                foreach (var item in autumnPageable.AutumnSort.OrderDescendingBy)
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
            return new AutumnPage<TEntity>(content, autumnPageable, count);
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