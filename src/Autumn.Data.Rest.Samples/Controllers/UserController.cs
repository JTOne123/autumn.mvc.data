using Autumn.Data.Rest.Controllers;
using Autumn.Data.Rest.Repositories;
using Microsoft.AspNetCore.Mvc;
using Autumn.Data.Rest.Samples.Entities;

namespace Autumn.Data.Rest.Samples.Controllers
{
    [Route("v1/users")]
    public class UserController : RestRepositoryControllerAsync<User, string>
    {
        public UserController(ICrudPageableRepositoryAsync<User, string> repository) : base(repository)
        {
            var a = repository;
        }
    }
}