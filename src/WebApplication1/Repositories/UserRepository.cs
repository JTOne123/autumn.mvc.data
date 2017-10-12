using System.Threading.Tasks;
using MongoDB.Driver;
using WebApplication1.Entities;
using Settings = WebApplication1.Configurations.Settings;

namespace WebApplication1.Repositories
{
    public class UserRepository : AbstractMongoRepository<User>, IUserRepository
    {
        public UserRepository(Settings settings) : base(settings)
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