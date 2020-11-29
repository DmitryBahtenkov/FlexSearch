using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Microsoft.AspNetCore.Mvc;
using SearchApi.Services;


namespace SearchApi.Controllers
{
    [Route("")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly GetOperations _getOperations;
        private readonly UpdateOperations _updateOperations;
        private readonly ObjectCreatorFacade _objectCreatorFacade;

        public IndexController(ObjectCreatorFacade objectCreatorFacade, GetOperations getOperations, UpdateOperations updateOperations)
        {
            _objectCreatorFacade = objectCreatorFacade;
            _getOperations = getOperations;
            _updateOperations = updateOperations;
        }

        [HttpGet]
        public async Task<IEnumerable<string>> GetDatabases()
        {
            return await _getOperations.GetDatabases();
        }
        
        [HttpGet("index/{dbname}")]
        public async Task<IEnumerable<string>> GetIndexes(string dbname)
        {
            return await _getOperations.GetIndexes(dbname);
        }
        
        [HttpGet("index/{dbname}/{index}/all")]
        public async Task<IEnumerable<string>> GetDocuments(string dbname, string index)
        {
            var docs = await _getOperations.GetDocuments(new IndexModel(dbname, index));
            var result = docs.Select(x => x.ToString());
            return result;
        }
        
        [HttpGet("index/{dbname}/{index}/{id}")]
        public async Task<string> GetDocument(string dbname, string index, int id)
        {
            var docs = await _getOperations.GetDocuments(new IndexModel(dbname, index));
            var result = docs.FirstOrDefault(x => x.Id == id);
            return result?.ToString();
        }

        [HttpPost("index/{dbname}/{index}/add")]
        public async Task<ActionResult> CreateIndex([FromBody] object obj, string dbname, string index)
        {
            await _objectCreatorFacade.CreateIndexAndAddObject(new IndexModel(dbname, index), obj);
            return StatusCode(201);
        }

        [HttpPut("index/{dbname}/{index}/rename")]
        public async Task<ActionResult> RenameIndex(string dbname, string index, string name)
        {
            await _updateOperations.RenameIndex(new IndexModel(dbname, index), name);
            return StatusCode(202);
        }
        [HttpDelete("index/{dbname}/delete")]
        public async Task<ActionResult> DeleteDatabase(string dbname)
        {
            try
            {
                await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/{dbname}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
        
        [HttpDelete("index/{dbname}/{index}/delete")]
        public async Task<ActionResult> DeleteIndex(string dbname, string index)
        {
            try
            {
                await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/{dbname}/{index}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}