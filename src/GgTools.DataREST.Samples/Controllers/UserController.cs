using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using GgTools.DataREST.Commons;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [Route("v1/users")]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("find-by-email")]
        public async Task<IActionResult> FindByEmail([FromQuery] string email)
        {
            var data = await _userRepository.FindByEmailAsync(email);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("")]
        public async Task<IActionResult> Find(Expression<Func<User,bool>> filter, IPageable pageable)
        {
            var page = await _userRepository.FindAsync(filter,pageable);
            if (page.TotalElements == page.NumberOfElements)
            {
                return Ok(page);
            }
            return StatusCode((int)HttpStatusCode.PartialContent, page);
        }
    }
}