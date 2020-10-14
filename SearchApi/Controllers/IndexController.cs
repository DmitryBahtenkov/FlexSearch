using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Storage.FileSystem;

namespace SearchApi.Controllers
{
    [Route("Index")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly GetDocumentsCommand _getDocumentsCommand;
        private readonly CreateDbCommand _createDbCommand;
        private readonly CreateIndexCommand _createIndexCommand;
        private readonly AddObjectToIndex _addObjectToIndex;

        public IndexController()
        {
            _getDocumentsCommand = new GetDocumentsCommand();
            _createDbCommand = new CreateDbCommand();
            _createIndexCommand = new CreateIndexCommand();
            _addObjectToIndex = new AddObjectToIndex();
        }
        [HttpGet]
        public async Task<IActionResult> GetDocuments(string dbname, string index)
        {
            var docs = await _getDocumentsCommand.Get(dbname, index);
            var result = docs
                .Select(x => new {Id = x.Id, Value = JsonConvert.SerializeObject(x.Value)});
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateIndex([FromBody] object obj, string dbname, string index)
        {
            var str = obj.ToString();
            _createDbCommand.CreateDb(dbname);
            _createIndexCommand.CreateIndex(dbname,index);
            await _addObjectToIndex.Add(dbname, index, str);
            return Ok();
        }
    }
}