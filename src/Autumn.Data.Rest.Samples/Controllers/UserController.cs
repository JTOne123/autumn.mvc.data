using System.Threading.Tasks;
using Autumn.Data.Rest.Controllers;
using Microsoft.AspNetCore.Mvc;
using Autumn.Data.Rest.Samples.Entities;
using Autumn.Data.Rest.Samples.Repositories;
using MongoDB.Bson;

namespace Autumn.Data.Rest.Samples.Controllers
{
    [Route("v1/users")]
    public class UserController : RestRepositoryControllerAsync<IUserRepository,User,ObjectId>
    {
      
        public UserController(IUserRepository userRepository):base(userRepository){ }

        [HttpGet("find-by-email")]
        public async Task<IActionResult> FindByEmail([FromQuery] string email)
        {
            var data = await Repository().FindByEmailAsync(email);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

    }
}