using System;
using System.IO;
using System.Linq;
using System.Text.Json;
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
        private readonly UserService _userService;
        private readonly DatabaseService _databaseService;

        public IndexController(ObjectCreatorFacade objectCreatorFacade, 
            GetOperations getOperations, 
            UpdateOperations updateOperations, 
            ILogger<IndexController> logger, 
            UserService userService, 
            DatabaseService databaseService)
        {
            _objectCreatorFacade = objectCreatorFacade;
            _getOperations = getOperations;
            _updateOperations = updateOperations;
            _logger = logger;
            _userService = userService;
            _databaseService = databaseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDatabases()
        {
            if (await _userService.CheckAuthorize(Request) is null)
                return Unauthorized();
            return Ok(await _databaseService.GetDatabases());
        }
        
        [HttpGet("index/{dbname}")]
        public async Task<IActionResult> GetIndexes(string dbname)
        {
            throw new NotImplementedException("Не реализовано!");
        }
        
        [HttpGet("index/{dbname}/{index}/all")]
        public async Task<IActionResult> GetDocuments(string dbname, string index)
        {
            return Ok(await _databaseService.GetAll(new IndexModel(dbname, index)));
        }
        
        [HttpGet("index/{dbname}/{index}/{id}")]
        public async Task<IActionResult> GetDocument(string dbname, string index, string id)
        {
            if (await _userService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized();
            var result = await _databaseService.FindById(new IndexModel(dbname, index), id);
            if (result is null)
                return NoContent();
            return Ok(new DocumentDto
            {
                Id = result.Id,
                Value = JsonDocument.Parse(result.Value.ToString())
            });
        }

        [HttpPost("index/{dbname}/{index}/add")]
        public async Task<IActionResult> CreateIndex([FromBody] object obj, string dbname, string index)
        {
            if (await _userService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized();
            try
            {
                var id = await _databaseService.Insert(new IndexModel(dbname, index), obj);
                return StatusCode(201, id);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: Create object {obj} in {dbname}/{index}, Error: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpPut("index/{dbname}/{index}/rename")]
        public async Task<IActionResult> RenameIndex(string dbname, string index, string name)
        {
            if (await _userService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized();
            try
            {
                await _databaseService.RenameIndex(new IndexModel(dbname, index), name);
                _logger.Log(LogLevel.Information, $"INFO: Rename index {dbname}/{index}. New name: {name}");
                return StatusCode(202);
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: index {index} not found");
                return BadRequest($"Не найдено индекса с именем {index}");
            }
        }
        
        [HttpPut("index/{dbname}/{index}/{id}/update")]
        public async Task<IActionResult> UpdateObject([FromBody] object obj,string dbname, string index, string id)
        {
            if (await _userService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized();
            try
            {
                await _databaseService.Update(new IndexModel(dbname, index), obj, id);
                _logger.Log(LogLevel.Information, $"INFO: Update object with id: {id} in {dbname}/{index}. New object: {obj}");
            }
            catch (FileNotFoundException ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: Object with id: {id} not found");
                return BadRequest($"Не существует записи с id: {id}");
            }
            return StatusCode(202);

        }
        
        [HttpDelete("index/{dbname}/delete")]
        public async Task<IActionResult> DeleteDatabase(string dbname)
        {
            if (await _userService.CheckAuthorize(Request, true) is null)
                return Unauthorized();
            try
            {
                await _databaseService.DeleteDatabase(dbname);
                _logger.Log(LogLevel.Information, $"INFO: Delete database {dbname}");
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR:  Database {dbname} not found");
                return NoContent();
            }

            return Ok();
        }
        
        [HttpDelete("index/{dbname}/{index}/delete")]
        public async Task<IActionResult> DeleteIndex(string dbname, string index)
        {
            if (await _userService.CheckAuthorize(Request, true) is null)
                return Unauthorized();
            try
            {
                await _databaseService.DeleteIndex(new IndexModel(dbname, index));
                _logger.Log(LogLevel.Information, $"INFO: Delete index {dbname}/{index}");
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: index {dbname}/{index} not found");
                return NoContent();
            }

            return Ok();
        }

        [HttpDelete("index/{dbname}/{index}/{id}/delete")]
        public async Task<IActionResult> DeleteObject(string dbname, string index, string id)
        {
            if (await _userService.CheckAuthorize(Request, true) is null)
                return Unauthorized();
            try
            {
                _logger.Log(LogLevel.Information, $"INFO: Delete object from {dbname}/{index} with id: {id}");
                //todo: refactor this
                var indexModel = new IndexModel(dbname, index);
                var model = await _databaseService.FindById(indexModel, id);
                await _databaseService.Delete(indexModel, model);
            }
            catch (FileNotFoundException ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: Object with id: {id} not found");
                return BadRequest($"Не существует записи с id: {id}");
            }

            return Ok();
        }
    }
}