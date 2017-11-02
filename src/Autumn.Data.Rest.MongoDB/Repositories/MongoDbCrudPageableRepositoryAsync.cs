using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Autumn.Data.Rest.Helpers;
using Autumn.Data.Rest.MongoDB.Entities;
using Autumn.Data.Rest.MongoDB.Helpers;
using Autumn.Data.Rest.Paginations;
using Autumn.Data.Rest.Repositories;
using MongoDB.Driver;

namespace Autumn.Data.Rest.MongoDB.Repositories
{
    public abstract class MongoDbCrudPageableRepositoryAsync<T,TId> : ICrudPageableRepositoryAsync<T,TId> 
        where T :class
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;
        private readonly ParameterExpression _parameter;
        private readonly FilterDefinitionBuilder<T> _filterDefinitionBuilder;
        private readonly PropertyInfo _propertyId;
        
        protected IMongoClient Client()
        {
            return _client;
        }

        protected IMongoDatabase Database()
        {
            return _database;
        }

        protected IMongoCollection<T> Collection()
        {
            return _collection;
        }

        protected MongoDbCrudPageableRepositoryAsync(string connectionString,string database)
        {
            _parameter = Expression.Parameter(typeof(T));
            _filterDefinitionBuilder=new FilterDefinitionBuilder<T>();
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(database);
            _propertyId = MongoDbHelper.GetId<T>();
            var collectionName = typeof(T).Name.ToLowerInvariant();
            var collectionAttribute = (BsonCollectionAttribute)
                typeof(T).GetCustomAttribute(typeof(BsonCollectionAttribute));
            if (collectionAttribute != null)
            {
                collectionName = collectionAttribute.CollectionName;
            }
            _collection = _database.GetCollection<T>(collectionName);
        }

        public async Task<T> FindOneAsync(TId id)
        {
            var propertyId = MongoDbHelper.GetId<T>();
            var where = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, propertyId),
                    Expression.Constant(id)
                )
            );
            return await Collection().Find(where).SingleOrDefaultAsync();
        }

        public async Task<Page<T>> FindAsync(Expression<Func<T, bool>> filter = null, Pageable<T> pageable = null)
        {
            var query = filter ?? CommonHelper.True<T>();

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
                            find = ((IOrderedFindFluent<T, T>) find).ThenBy(item);
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
                            find = ((IOrderedFindFluent<T, T>) find).ThenByDescending(item);
                        }
                    }
                }
            }

            var content = await find.ToListAsync();
            return new Page<T>(content, pageable, count);
        }

        public async Task<T> CreateAsync(T entity)
        {
            await Collection().InsertOneAsync(entity);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity, TId id)
        {
            var update = new ObjectUpdateDefinition<T>(entity);
            var where = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, _propertyId),
                    Expression.Constant(id)
                )
            );

            var filter = _filterDefinitionBuilder.Where(where);
            return await Collection().FindOneAndUpdateAsync(filter, update);
        }

        public async Task<T> DeleteAsync(TId id)
        {
            var where = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, _propertyId),
                    Expression.Constant(id)
                )
            );

            var filter = _filterDefinitionBuilder.Where(where);
            return await Collection().FindOneAndDeleteAsync(filter);
        }
    }
}