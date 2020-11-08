using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Searcher;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SearchApi.Controllers
{
    [ApiController]
    [Route("search")]
    public class SearchController : ControllerBase
    {
        private readonly Searcher _searcher;

        public SearchController()
        {
            _searcher = new Searcher();
        }

        [HttpGet("/fulltext/{dbname}/{index}")]
        public async Task<IActionResult> SearchIntersect([FromBody] BaseSearchModel searchModel, string dbname, string index)
        {
            var docs = await _searcher.SearchIntersect(new IndexModel(dbname, index), searchModel, "en");
            var result = docs.Select(x => new
            {
                x.Id,
                Value = JsonConvert.SerializeObject(x.Value)
            });

            return Ok(result);
        }

        [HttpGet("/match/{dbname}/{index}")]
        public async Task<IActionResult> SearchMatch([FromBody] BaseSearchModel searchModel, string dbname, string index)
        {
            var docs = await _searcher.SearchMatch(new IndexModel(dbname, index), searchModel);
            var result = docs.Select(x => new
            {
                x.Id,
                Value = JsonConvert.SerializeObject(x.Value)
            });

            return Ok(result);
        }

    }
}