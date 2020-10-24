using System.Linq;
using System.Threading.Tasks;
using Core.Searcher;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SearchApi.Controllers
{
    [ApiController]
    [Route("Search")]
    public class SearchController : ControllerBase
    {
        private readonly Searcher _searcher;

        public SearchController()
        {
            _searcher = new Searcher();
        }

        [HttpGet]
        public async Task<IActionResult> SearchIntersect(string dbname, string index, string key, string text)
        {
            var docs = await _searcher.Search(dbname, index, key, text);
            var result = docs.Select(x => new
            {
                x.Id,
                Value = JsonConvert.SerializeObject(x.Value)
            });

            return Ok(result);
        }
    }
}