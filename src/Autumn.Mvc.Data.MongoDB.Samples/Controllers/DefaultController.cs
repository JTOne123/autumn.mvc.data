using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Data.Repositories;
using Microsoft.AspNetCore.Http;

namespace Autumn.Mvc.Data.MongoDB.Samples.Controllers
{
    public class DefaultController<TEntity, TEntityPost, TEntityPut, TKey>: 
        RepositoryControllerAsync<TEntity, TEntityPost, TEntityPut, TKey>
        where TEntity: class 
        where TEntityPost: class
        where TEntityPut: class
    {
        public DefaultController(ICrudPageableRepositoryAsync<TEntity, TKey> repository, AutumnSettings settings, IHttpContextAccessor httpContextAccessor) : base(repository, settings, httpContextAccessor)
        {
            int i = 0;
        }
    }
}