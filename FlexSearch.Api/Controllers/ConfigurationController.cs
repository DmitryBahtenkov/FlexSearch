using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Core.Configuration;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SearchApi.Dtos;
using SearchApi.Services;

namespace SearchApi.Controllers
{
    [Route("configuration")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            if(await UserService.CheckAuthorize(Request, true) is not null)
                return Ok(await ConfigurationService.Get());
            return Unauthorized();
        }
        
        [HttpPost]
        public async Task<IActionResult> SetConfig([FromBody] ConfigurationModel configurationModel)
        {
            if (await UserService.CheckAuthorize(Request, true) is null)
                return Unauthorized();
            try
            {
                await ConfigurationService.Set(configurationModel);
            }
            catch (ValidationException validationException)
            {
                return BadRequest(new ErrorDto(ErrorsType.ValidationError, validationException.Message));
            }
            catch (Exception exception)
            {
                return BadRequest(new ErrorDto(ErrorsType.SystemError, exception.Message));
            }

            return Ok();
        }
        
        [HttpPost("default")]
        public async Task<IActionResult> SetDefault()
        {
            if (await UserService.CheckAuthorize(Request, true) is null)
                return Unauthorized();
            await ConfigurationService.SetDefault();
            return Ok();
        }
    }
}