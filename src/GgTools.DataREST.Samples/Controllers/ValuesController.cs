using Microsoft.AspNetCore.Mvc;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {

        private readonly IUserRepository _userRepository;

        public ValuesController(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }
        
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new string[] {"value1", "value2"});
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet("find-by-email")]
        public IActionResult GetByEmail([FromQuery] string email)
        {
            var user = _userRepository.FindByEmailAsync(email);
            if (user != null)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}