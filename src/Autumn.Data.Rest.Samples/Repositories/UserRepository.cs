using System.Threading.Tasks;
using Autumn.Data.Rest.MongoDB.Repositories;
using MongoDB.Driver;
using Autumn.Data.Rest.Samples.Entities;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using Settings = Autumn.Data.Rest.Samples.Configurations.Settings;

namespace Autumn.Data.Rest.Samples.Repositories
{
    public class UserRepository : MongoDbCrudPageableRepositoryAsync<User,string>, IUserRepository
    {
        public UserRepository(Settings settings) : base(settings.ConnectionString,settings.DatabaseName)
        {
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await Collection()
                .Find(x => x.Email == email)
                .SingleOrDefaultAsync();
        }
    }
}