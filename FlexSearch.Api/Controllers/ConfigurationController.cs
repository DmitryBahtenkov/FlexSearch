using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Core.Configuration;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace SearchApi.Controllers
{
    [Route("configuration")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            return Ok(await ConfigurationService.Get());
        }
        
        [HttpPost]
        public async Task<IActionResult> SetConfig([FromBody] ConfigurationModel configurationModel)
        {
            try
            {
                await ConfigurationService.Set(configurationModel);
            }
            catch (ValidationException exception)
            {
                return BadRequest(exception.Message);
            }

            return Ok();
        }
        
        [HttpPost("default")]
        public async Task<IActionResult> SetDefault()
        {
            await ConfigurationService.SetDefault();
            return Ok();
        }
    }
}