using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Autumn.Data.Rest.Commons;
using Autumn.Data.Rest.MongoDB.Entities;
using Autumn.Data.Rest.Repositories;
using Autumn.Data.Rest.Rsql;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Autumn.Data.Rest.MongoDB.Repositories
{
    public abstract class MongoDbCrudPageableRepositoryAsync<T,TId> : ICrudPageableRepositoryAsync<T,TId> 
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;
        private readonly ParameterExpression _parameter;
        private readonly FilterDefinitionBuilder<T> _filterDefinitionBuilder;
        private static readonly Dictionary<Type, PropertyInfo> Ids=new Dictionary<Type, PropertyInfo>();

        /// <summary>
        /// return propertyinfo BsonIdof entity class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
#pragma warning disable 693
        private static PropertyInfo GetProperyId<T>()
#pragma warning restore 693
        {
            lock (Ids)
            {
                if (Ids.ContainsKey(typeof(T))) return Ids[typeof(T)];
                var propertyInfo = typeof(T).GetProperties().Single(p => p.GetCustomAttribute<BsonIdAttribute>() != null);
                Ids.Add(typeof(T),propertyInfo);
                return Ids[typeof(T)];
            }
        }
        
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
            var propertyId = GetProperyId<T>();
            var where = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, propertyId),
                    Expression.Constant(id)
                )
            );
            return await Collection().Find(where).SingleOrDefaultAsync();
        }

        public async Task<IPage<T>> FindAsync(Expression<Func<T, bool>> filter=null, IPageable pageable=null)
        {
            var query = filter ?? RsqlHelper.True<T>();

            var count = (int) await Collection().CountAsync(query);
            var find = Collection().Find(query);

            if (pageable != null)
            {
                var offset = pageable.PageNumber * pageable.PageSize;
                var limit = pageable.PageSize;
                find = find.Skip(offset).Limit(limit);
                if (pageable.Sort.Count > 0)
                {
                    
                    var sortBuilder = Builders<T>.Sort;
                    SortDefinition<T> current = null;
                    foreach (var sort in pageable.Sort)
                    {
                        if (sort.IsAscending)
                        {
                            current = current == null
                                ? sortBuilder.Ascending(sort.Property)
                                : current.Ascending(sort.Property);
                        }
                        else
                        {
                            current = current == null
                                ? sortBuilder.Descending(sort.Property)
                                : current.Descending(sort.Property);
                        }
                    }
                    find = find.Sort(current);
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

        public async Task<T> UpdateAsync(T entity)
        {
            var update = new ObjectUpdateDefinition<T>(entity);
            var propertyId = GetProperyId<T>();
            var where = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, propertyId),
                    Expression.Constant(propertyId.GetValue(entity, null))
                )
            );

            var filter = _filterDefinitionBuilder.Where(where);
            return await Collection().FindOneAndUpdateAsync(filter, update);
        }

        public async Task<T> UpdateAsync(T entity, TId id)
        {
            var update = new ObjectUpdateDefinition<T>(entity);
            var propertyId = GetProperyId<T>();
            var where = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, propertyId),
                    Expression.Constant(id)
                )
            );

            var filter = _filterDefinitionBuilder.Where(where);
            return await Collection().FindOneAndUpdateAsync(filter, update);
        }

        public async Task<T> DeleteAsync(TId id)
        {
            var propertyId = GetProperyId<T>();
            var where = Expression.Lambda<Func<T, bool>>(
                Expression.Equal(
                    Expression.Property(_parameter, propertyId),
                    Expression.Constant(id)
                )
            );

            var filter = _filterDefinitionBuilder.Where(where);
            return await Collection().FindOneAndDeleteAsync(filter);
        }
    }
}