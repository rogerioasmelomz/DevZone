using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Models;

namespace TestAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        static List<UserModel> _users;

        public UsersController()
        {

            _users = new List<UserModel>();

            _users.Add(new UserModel() { Id = 1, Nome = "Francisco Coelho", Email = "fc@teste.com" });
            _users.Add(new UserModel() { Id = 2, Nome = "José Pinto", Email = "jp@teste.com" });
            _users.Add(new UserModel() { Id = 3, Nome = "Manuel José", Email = "mj@teste.com" });
            _users.Add(new UserModel() { Id = 4, Nome = "Abel Matos", Email = "am@teste.com" });
            _users.Add(new UserModel() { Id = 5, Nome = "Maria Melo", Email = "mm@teste.com" });

        }

        [HttpGet]
        public string Get()
        {
            return "Users API 1.0";
        }


        [HttpGet("{id}")]
        public IActionResult Get(int Id)
        {

            var user = _users.FirstOrDefault(i => i.Id == Id);

            if (user == null)
            {
                return NotFound("Utilizador não existe!");
            }

            return Ok(user);

        }

        [HttpPost]
        public IActionResult Post([FromBody] UserModel NewUser)
        {

            //int newId = _users.Count + 1;

            //NewUser.Id = newId;

            _users.Add(NewUser);

            return Ok(string.Format("O utilizador '{0}' foi adicionado", NewUser.Id));
        }

        [HttpPut("{id}")]
        public string Put(int Id, [FromBody] UserModel User)
        {

            return string.Format("Utilizador '{0}' foi actualizado", Id);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int Id)
        {

            var user = _users.FirstOrDefault(i => i.Id == Id);

            if (user == null)
            {
                return NotFound("Utilizador não existe!");
            }



            return Ok(string.Format ("O utilizador '{0}' foi apagado!", Id));
        }
    }
}
