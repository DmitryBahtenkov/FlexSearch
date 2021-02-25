using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SearchApi.Services;

namespace SearchApi.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateUser([FromBody] UserModel userModel)
        {
            if (await UserService.CheckAuthorize(Request, true) is not null)
            {
                await UserService.CreateUser(userModel);
                return StatusCode(201);
            }
            
            return Unauthorized();
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUsersNoPass()
        {
            if (await UserService.CheckAuthorize(Request) is not null)
            {
                return Ok(await UserService.GetUsersNoPassword());
            }

            return Unauthorized();
        }
        
        [HttpGet("pass")]
        public async Task<IActionResult> GetUsers()
        {
            if (await UserService.CheckAuthorize(Request, true) is not null)
            {
                return Ok(await UserService.GetUsers());
            }

            return Unauthorized();
        }

        [HttpGet("{user}")]
        public async Task<IActionResult> GetUser(string user)
        {
            if (await UserService.CheckAuthorize(Request, true) is not null)
            {
                return Ok(await UserService.GetUser(user));
            }

            return Unauthorized();
        }

        [HttpPut("{user}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel userModel, string user)
        {
            if (await UserService.CheckAuthorize(Request, true) is not null)
            {
                if (user == "root" && (userModel.Database != "all" || userModel.UserName != "root"))
                    return BadRequest("You can not change the name and rights of the root user");
                
                await UserService.UpdateUser(user, userModel);
                return Ok();
            }

            return Unauthorized();
        }
        
        [HttpDelete("{user}")]
        public async Task<IActionResult> DeleteUser(string user)
        {
            if (await UserService.CheckAuthorize(Request, true) is not null)
            {
                if (user == "root")
                    return BadRequest("You can not delete root user");
                
                await UserService.DeleteUser(user);
                return Ok();
            }

            return Unauthorized();
        }
    }
}