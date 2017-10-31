using System.Threading.Tasks;
using Autumn.Data.Rest.Repositories;
using MongoDB.Bson;
using Autumn.Data.Rest.Samples.Entities;

namespace Autumn.Data.Rest.Samples.Repositories
{
    public interface IUserRepository : ICrudPageableRepositoryAsync<User,ObjectId>
    {
        Task<User> FindByEmailAsync(string email);
    }
}