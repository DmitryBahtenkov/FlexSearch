using Microsoft.AspNetCore.Mvc;

namespace SearchApi.Controllers
{
    [Route("")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public string GetHello()
        {
            return "hello";
        }
    }
}