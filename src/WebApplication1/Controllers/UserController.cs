using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Commons;
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
        public async Task<IActionResult> Find(IPageable pageable)
        {
            var page = await _userRepository.FindAsync(pageable);
            if (page.TotalElements == page.NumberOfElements)
            {
                return Ok(page);
            }
            return StatusCode((int)HttpStatusCode.PartialContent, page);
        }
    }
}