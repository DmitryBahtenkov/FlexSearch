using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using SearchApi.Services;

namespace SearchApi.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> CreateUser([FromBody] UserModel userModel)
        {
            if (await _userService.CheckAuthorize(Request, true) is not null)
            {
                await _userService.UserRepository.CreateUser(userModel);
                return StatusCode(201);
            }

            return Unauthorized();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUsersNoPass()
        {
            if (await _userService.CheckAuthorize(Request) is not null)
            {
                return Ok(await _userService.UserRepository.GetUsersNoPassword());
            }

            return Unauthorized();
        }
        
        [HttpGet("all/pass")]
        public async Task<IActionResult> GetUsers()
        {
            if (await _userService.CheckAuthorize(Request, true) is not null)
            {
                return Ok(await _userService.UserRepository.GetUsers());
            }

            return Unauthorized();
        }

        [HttpGet("{user}")]
        public async Task<IActionResult> GetUser(string user)
        {
            if (await _userService.CheckAuthorize(Request, true) is not null)
            {
                return Ok(await _userService.UserRepository.GetUser(user));
            }

            return Unauthorized();
        }

        [HttpPut("update/{user}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel userModel, string user)
        {
            if (await _userService.CheckAuthorize(Request, true) is not null)
            {
                if (user == "root" && (userModel.Database != "all" || userModel.UserName != "root"))
                    return BadRequest("You can not change the name and rights of the root user");
                
                await _userService.UserRepository.UpdateUser(user, userModel);
                return Ok();
            }

            return Unauthorized();
        }
        
        [HttpDelete("delete/{user}")]
        public async Task<IActionResult> DeleteUser(string user)
        {
            if (await _userService.CheckAuthorize(Request, true) is not null)
            {
                if (user == "root")
                    return BadRequest("You can not delete root user");
                
                await _userService.UserRepository.DeleteUser(user);
                return Ok();
            }

            return Unauthorized();
        }
    }
}