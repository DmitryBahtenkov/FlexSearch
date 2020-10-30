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

        [HttpGet("/text/{dbname}/{index}")]
        public async Task<IActionResult> SearchIntersect([FromBody] BaseSearchModel searchModel, string dbname, string index)
        {
            var docs = await _searcher.Search(dbname, index, searchModel);
            var result = docs.Select(x => new
            {
                x.Id,
                Value = JsonConvert.SerializeObject(x.Value)
            });

            return Ok(result);
        }
    }
}