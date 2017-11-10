using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models;
using Autumn.Mvc.Data.Models.Helpers;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.MongoDB.Configurations;
using Autumn.Mvc.Data.Repositories;
using MongoDB.Driver;

namespace Autumn.Mvc.Data.MongoDB.Repositories
{
    public class MongoCrudPageableRepositoryAsync<TEntity,TKey> : ICrudPageableRepositoryAsync<TEntity,TKey> 
        where TEntity :class
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<TEntity> _collection;
        private readonly ParameterExpression _parameter;
        private readonly FilterDefinitionBuilder<TEntity> _filterDefinitionBuilder;
        private readonly PropertyInfo _propertyId;
        
        protected IMongoClient Client()
        {
            return _client;
        }

        protected IMongoDatabase Database()
        {
            return _database;
        }

        protected IMongoCollection<TEntity> Collection()
        {
            return _collection;
        }

        public MongoCrudPageableRepositoryAsync(AutumnMongoSettings settings)
        {
            _parameter = Expression.Parameter(typeof(TEntity));
            _filterDefinitionBuilder=new FilterDefinitionBuilder<TEntity>();
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);
            _propertyId = IdAttribute.GetOrRegisterId<TEntity>();
            var collectionName = typeof(TEntity).Name.ToLowerInvariant();
            var collectionAttribute = (EntityAttribute)
                typeof(TEntity).GetCustomAttribute(typeof(EntityAttribute));
            if (collectionAttribute != null)
            {
                collectionName = collectionAttribute.Name;
            }
            _collection = _database.GetCollection<TEntity>(collectionName);
        }

        public async Task<TEntity> FindOneAsync(TKey id)
        {
            var propertyId = IdAttribute.GetOrRegisterId<TEntity>();
            var where = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, propertyId),
                    Expression.Constant(id)
                )
                ,_parameter
            );
            return await Collection().Find(where).SingleOrDefaultAsync();
        }

        public async Task<Page<TEntity>> FindAsync(Expression<Func<TEntity, bool>> filter = null, Pageable<TEntity> pageable = null)
        {
            var query = filter ?? CommonHelper.True<TEntity>();

            var count = (int) await Collection().CountAsync(query);
            var find = Collection().Find(query);

            if (pageable != null)
            {
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
            }

            var content = await find.ToListAsync();
            return new Page<TEntity>(content, pageable, count);
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            await Collection().InsertOneAsync(entity);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, TKey id)
        {
            var update = new ObjectUpdateDefinition<TEntity>(entity);
            var where = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, _propertyId),
                    Expression.Constant(id)
                ),
                _parameter
            );

            var filter = _filterDefinitionBuilder.Where(where);
            var result = await Collection().ReplaceOneAsync(filter, entity);
            return entity;
        }

        public async Task<TEntity> DeleteAsync(TKey id)
        {
            var where = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, _propertyId),
                    Expression.Constant(id)
                )
                ,
                _parameter
            );

            var filter = _filterDefinitionBuilder.Where(where);
            return await Collection().FindOneAndDeleteAsync(filter);
        }
    }
}