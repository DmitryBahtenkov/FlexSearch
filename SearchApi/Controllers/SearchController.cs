using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Searcher;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SearchApi.Services;

namespace SearchApi.Controllers
{
    [ApiController]
    [Route("search")]
    public class SearchController : ControllerBase
    {
        private readonly SearcherService _searcher;

        public SearchController(SearcherService searcher)
        {
            _searcher = searcher;
        }

        [HttpGet("/search/{dbname}/{index}")]
        public async Task<IActionResult> SearchIntersect([FromBody] BaseSearchModel searchModel, string dbname, string index)
        {
            var docs = await _searcher.Search(new IndexModel(dbname, index), searchModel);
            var result = docs.Select(x => new
            {
                x.Id,
                Value = JsonConvert.SerializeObject(x.Value)
            });

            return Ok(result);
        }
        
    }
}