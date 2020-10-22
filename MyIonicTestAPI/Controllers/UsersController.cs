using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyIonicTestAPI.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class UsersController : Controller
    {
        private List<User> _users;

        public UsersController()
        {

            _users = new List<User>();

            _users.Add(new User() { Id = 1, Name = "User1" });
            _users.Add(new User() { Id = 2, Name = "User2" });
            _users.Add(new User() { Id = 3, Name = "User3" });
            _users.Add(new User() { Id = 4, Name = "User4" });

        }

        [HttpGet]
        public string Get() {

            return "API 1.0";    
        }

        [HttpGet("{id}")]
        public IActionResult Get(int Id) {

            var user = _users.FirstOrDefault(i=> i.Id==Id);

            if (user == null) {
                return NotFound("Utilizador não existe!");
            }

            return Ok(user);

        }
    }
}
