using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Microsoft.AspNetCore.Mvc;
using SearchApi.Services;


namespace SearchApi.Controllers
{
    [Route("index")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly GetOperations _getOperations;
        private readonly ObjectCreatorFacade _objectCreatorFacade;

        public IndexController(ObjectCreatorFacade objectCreatorFacade)
        {
            _objectCreatorFacade = objectCreatorFacade;
            _getOperations = new GetOperations();
        }
        
        [HttpGet("{dbname}/{index}/all")]
        public async Task<IEnumerable<string>> GetDocuments(string dbname, string index)
        {
            var docs = await _getOperations.GetDocuments(new IndexModel(dbname, index));
            var result = docs.Select(x => x.ToString());
            return result;
        }
        
        [HttpGet("{dbname}/{index}/{id}")]
        public async Task<string> GetDocument(string dbname, string index, int id)
        {
            var docs = await _getOperations.GetDocuments(new IndexModel(dbname, index));
            var result = docs.FirstOrDefault(x => x.Id == id);
            return result?.ToString();
        }

        [HttpPost("{dbname}/{index}/add")]
        public async Task<ActionResult> CreateIndex([FromBody] object obj, string dbname, string index)
        {
            await _objectCreatorFacade.CreateIndexAndAddObject(new IndexModel(dbname, index), obj);
            return StatusCode(201);
        }
    }
}