using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<IndexController> _logger;

        public IndexController(ObjectCreatorFacade objectCreatorFacade, 
            GetOperations getOperations, 
            UpdateOperations updateOperations, 
            ILogger<IndexController> logger)
        {
            _objectCreatorFacade = objectCreatorFacade;
            _getOperations = getOperations;
            _updateOperations = updateOperations;
            _logger = logger;
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
        public async Task<string> GetDocument(string dbname, string index, string id)
        {
            var docs = await _getOperations.GetDocuments(new IndexModel(dbname, index));
            var result = docs.FirstOrDefault(x => x.Id.ToString() == id);
            return result?.ToString();
        }

        [HttpPost("index/{dbname}/{index}/add")]
        public async Task<ActionResult> CreateIndex([FromBody] object obj, string dbname, string index)
        {
            try
            {
                await _objectCreatorFacade.CreateIndexAndAddObject(new IndexModel(dbname, index), obj);
                _logger.Log(LogLevel.Information, $"INFO: |{DateTime.Now}| Create object in {{dbname}}/{{index}}, Object: {{obj}}");
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: |{DateTime.Now}| Create object {obj} in {dbname}/{index}, Error: {ex.Message}");
                return StatusCode(500);
            }

        }

        [HttpPut("index/{dbname}/{index}/rename")]
        public async Task<ActionResult> RenameIndex(string dbname, string index, string name)
        {
            try
            {
                await _updateOperations.RenameIndex(new IndexModel(dbname, index), name);
                _logger.Log(LogLevel.Information, $"INFO: |{DateTime.Now}| Rename index {dbname}/{index}. New name: {name}");
                return StatusCode(202);
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: |{DateTime.Now}| index {index} not found");
                return BadRequest($"Не найдено индекса с именем {index}");
            }

        }
        
        [HttpPut("index/{dbname}/{index}/{id}/update")]
        public async Task<ActionResult> UpdateObject([FromBody] object obj,string dbname, string index, string id)
        {
            try
            {
                await _objectCreatorFacade.UpdateObjectAndIndexing(new IndexModel(dbname, index), id, obj);
                _logger.Log(LogLevel.Information, $"INFO: |{DateTime.Now}| Update object with id: {id} in {dbname}/{index}. New object: {obj}");
            }
            catch (FileNotFoundException ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: |{DateTime.Now}| Object with id: {id} not found");
                return BadRequest($"Не существует записи с id: {id}");
            }
            return StatusCode(202);

        }
        
        [HttpDelete("index/{dbname}/delete")]
        public async Task<ActionResult> DeleteDatabase(string dbname)
        {
            try
            {
                await DeleteOperations.DeleteDatabase(dbname);
                _logger.Log(LogLevel.Information, $"INFO: |{DateTime.Now}| Delete database {dbname}");
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: |{DateTime.Now}| Database {dbname} not found");
                return NoContent();
            }

            return Ok();
        }
        
        [HttpDelete("index/{dbname}/{index}/delete")]
        public async Task<ActionResult> DeleteIndex(string dbname, string index)
        {
            try
            {
                await DeleteOperations.DeleteIndex(new IndexModel(dbname, index));
                _logger.Log(LogLevel.Information, $"INFO: |{DateTime.Now}| Delete index {dbname}/{index}");
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: |{DateTime.Now}| index {dbname}/{index} not found");
                return NoContent();
            }

            return Ok();
        }

        [HttpDelete("index/{dbname}/{index}/{id}/delete")]
        public async Task<ActionResult> DeleteObject(string dbname, string index, string id)
        {
            try
            {
                _logger.Log(LogLevel.Information, $"INFO: |{DateTime.Now}| Delete object from {dbname}/{index} with id: {id}");
                await DeleteOperations.DeleteObjectById(new IndexModel(dbname, index), id);
            }
            catch (FileNotFoundException ex)
            {
                _logger.Log(LogLevel.Error, "ERROR: |{DateTime.Now}| Object with id: {id} not found");
                return BadRequest($"Не существует записи с id: {id}");
            }

            return Ok();
        }
    }
}