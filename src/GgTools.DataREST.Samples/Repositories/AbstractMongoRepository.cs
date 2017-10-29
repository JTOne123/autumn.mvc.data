using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using GgTools.DataREST.Commons;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using WebApplication1.Configurations;

namespace WebApplication1.Repositories
{
    public abstract class AbstractMongoRepository<T> : IPageableRepository<T,ObjectId> where T : AbstractEntity
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;
        private static Expression<Func<T, bool>> True<T> ()  { return f => true;  }
        private static Expression<Func<T, bool>> False<T> () { return f => false; }

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

        protected AbstractMongoRepository(Settings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);
            var collectionName = typeof(T).Name.ToLowerInvariant();
            var collectionAttribute = (BsonCollectionAttribute)
                typeof(T).GetCustomAttribute(typeof(BsonCollectionAttribute));
            if (collectionAttribute != null)
            {
                collectionName = collectionAttribute.CollectionName;
            }

            _collection = _database.GetCollection<T>(collectionName);
        }

        public async Task<T> FindOneAsync(ObjectId id)
        {
            return await Collection()
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<IPage<T>> FindAsync(Expression<Func<T, bool>> filter=null, IPageable pageable=null)
        {

            var query = filter ?? True<T>();

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
    }
}