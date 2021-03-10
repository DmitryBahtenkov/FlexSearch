using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Core.Exceptions;
using Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SearchApi.Dtos;
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
                try
                {
                    await UserService.CreateUser(userModel);
                }
                catch (ExistingUserException userException)
                {
                    return BadRequest(new ErrorDto(ErrorsType.SyntaxError, userException.Message));
                }
                catch (ValidationException validationException)
                {
                    return BadRequest(new ErrorDto(ErrorsType.SyntaxError, validationException.Message));
                }
                return StatusCode(201);
            }
            
            return Unauthorized(ErrorDto.GetAuthError());
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUsersNoPass()
        {
            if (await UserService.CheckAuthorize(Request) is not null)
            {
                return Ok(await UserService.GetUsersNoPassword());
            }

            return Unauthorized(ErrorDto.GetAuthError());
        }
        
        [HttpGet("pass")]
        public async Task<IActionResult> GetUsers()
        {
            if (await UserService.CheckAuthorize(Request, true) is not null)
            {
                return Ok(await UserService.GetUsers());
            }

            return Unauthorized(ErrorDto.GetAuthError());
        }

        [HttpGet("{user}")]
        public async Task<IActionResult> GetUser(string user)
        {
            if (await UserService.CheckAuthorize(Request, true) is not null)
            {
                return Ok(await UserService.GetUser(user));
            }

            return Unauthorized(ErrorDto.GetAuthError());
        }

        [HttpPut("{user}")]
        public async Task<IActionResult> UpdateUser([FromBody] UserModel userModel, string user)
        {
            if (await UserService.CheckAuthorize(Request, true) is not null)
            {
                if (user == "root" && (userModel.Database != "all" || userModel.UserName != "root"))
                    return BadRequest(new ErrorDto(ErrorsType.ValidationError, "You can not change the name and rights of the root user"));

                try
                {
                    await UserService.UpdateUser(user, userModel);
                }
                catch (UserNotFoundException userEx)
                {
                    return BadRequest(new ErrorDto(ErrorsType.SyntaxError, userEx.Message));
                }
                catch (ValidationException validationEx)
                {
                    return BadRequest(new ErrorDto(ErrorsType.SyntaxError, validationEx.Message));
                }
                return Ok();
            }

            return Unauthorized(ErrorDto.GetAuthError());
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

            return Unauthorized(ErrorDto.GetAuthError());
        }
    }
}