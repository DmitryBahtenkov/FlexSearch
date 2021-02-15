using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Searcher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SearchApi.Services;

namespace SearchApi.Controllers
{
    [ApiController]
    [Route("search")]
    public class SearchController : ControllerBase
    {
        private readonly SearcherService _searcher;
        private readonly ILogger<SearchController> _logger;
        private readonly UserService _userService;

        public SearchController(SearcherService searcher, 
            ILogger<SearchController> logger, 
            UserService userService)
        {
            _searcher = searcher;
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("/search/{dbname}/{index}")]
        public async Task<IActionResult> Search([FromBody] BaseSearchModel searchModel, string dbname, string index)
        {
            if (await _userService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized();
            try
            {
                var docs = await _searcher.Search(new IndexModel(dbname, index), searchModel);
                var result = docs.Select(x => new
                {
                    x.Id,
                    Value = JsonConvert.SerializeObject(x.Value)
                });

                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: Search {searchModel.Type} - {searchModel.Key}:{searchModel.Term} in {dbname}/{index}, Error: {ex.Message}");
                return BadRequest();
            }
        }
        
    }
}