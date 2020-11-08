using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SearchApi.System;


namespace SearchApi.Controllers
{
    [Route("index")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly GetDocumentsCommand _getDocumentsCommand;
        private readonly CoreFacade _coreFacade;

        public IndexController()
        {
            _getDocumentsCommand = new GetDocumentsCommand();
            _coreFacade = new CoreFacade();
        }
        
        [HttpGet("{dbname}/{index}/all")]
        public async Task<IEnumerable<string>> GetDocuments(string dbname, string index)
        {
            var docs = await _getDocumentsCommand.Get(dbname, index);
            var result = docs.Select(x => x.ToString());
            return result;
        }
        
        [HttpGet("{dbname}/{index}/{id}")]
        public async Task<string> GetDocument(string dbname, string index, int id)
        {
            var docs = await _getDocumentsCommand.Get(dbname, index);
            var result = docs.FirstOrDefault(x => x.Id == id);
            return result?.ToString();
        }

        [HttpPost("{dbname}/{index}/add")]
        public async Task<ActionResult> CreateIndex([FromBody] object obj, string dbname, string index)
        {
            await _coreFacade.CreateAll(dbname, index, obj);
            return StatusCode(201);
        }
    }
}