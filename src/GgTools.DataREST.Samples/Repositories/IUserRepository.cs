using System.Threading.Tasks;
using MongoDB.Bson;
using WebApplication1.Entities;

namespace WebApplication1.Repositories
{
    public interface IUserRepository : IPageableRepository<User,ObjectId>
    {
        Task<User> FindByEmailAsync(string email);
    }
}