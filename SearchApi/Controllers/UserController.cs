using System;
using System.Net;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace SearchApi.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        [HttpPost("add")]
        public async Task<IActionResult> CreateUser([FromBody] UserModel userModel)
        {
            throw new NotImplementedException();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUsers()
        {
            throw new NotImplementedException();
        }

        [HttpGet("{user}")]
        public async Task<IActionResult> GetUser(string user)
        {
            throw new NotImplementedException();
        }

        [HttpPut("update/{user}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel userModel, string user)
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete("delete/{user}")]
        public async Task<IActionResult> DeleteUser(string user)
        {
            throw new NotImplementedException();
        }
    }
}