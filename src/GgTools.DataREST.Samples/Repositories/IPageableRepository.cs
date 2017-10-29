using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GgTools.DataREST.Commons;

namespace WebApplication1.Repositories
{
    public interface IPageableRepository<T,Pkey>
    {
        Task<T> FindOneAsync(Pkey id);

        Task<IPage<T>> FindAsync(Expression<Func<T, bool>> filter=null, IPageable pageable=null);
    }
}