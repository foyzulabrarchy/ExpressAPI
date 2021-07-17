

using ExpressAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ExpressAPI.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IJWTAuthenticationManager jWTAuthenticationManager;


        public HomeController(IJWTAuthenticationManager jWTAuthenticationManager)
        {
            this.jWTAuthenticationManager = jWTAuthenticationManager;
        }

        // GET: api/<NameController>
        [HttpGet("Getinfo")]
        [Authorize]
        public IEnumerable<string> Get()
        {

            return new string[] { "value1", "value2" };
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]

        [Authorize]
        public IActionResult Authenticate([FromBody] UserDTO user)
        {
            var token = jWTAuthenticationManager.Authenticate(user.Username, user.Password).ToString();

            if (token == null)
                return Unauthorized();

            return Ok(token);
        }

    }
}
