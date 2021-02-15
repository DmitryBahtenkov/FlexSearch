using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Core.Storage.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SearchApi.Mappings;
using SearchApi.Services;


namespace SearchApi.Controllers
{
    [Route("")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly ILogger<IndexController> _logger;
        private readonly UserService _userService;

        public IndexController(
            ILogger<IndexController> logger, 
            UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDatabases()
        {
            if (await _userService.CheckAuthorize(Request) is null)
                return Unauthorized();
            return Ok(await DatabaseService.GetDatabases());
        }
        
        [HttpGet("index/{dbname}")]
        public async Task<IActionResult> GetIndexes(string dbname)
        {
            throw new NotImplementedException("Не реализовано!");
        }
        
        [HttpGet("index/{dbname}/{index}/all")]
        public async Task<IActionResult> GetDocuments(string dbname, string index)
        {
            return Ok(DocumentMapper.MapToDtos(await DatabaseService.GetAll(new IndexModel(dbname, index))));
        }
        
        [HttpGet("index/{dbname}/{index}/{id}")]
        public async Task<IActionResult> GetDocument(string dbname, string index, string id)
        {
            if (await _userService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized();
            var result = await DatabaseService.FindById(new IndexModel(dbname, index), id);
            if (result is null)
                return NoContent();
            return Ok(DocumentMapper.MapToDto(result));
        }

        [HttpPost("index/{dbname}/{index}/add")]
        public async Task<IActionResult> CreateIndex([FromBody] object obj, string dbname, string index)
        {
            if (await _userService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized();
            try
            {
                var id = await DatabaseService.Insert(new IndexModel(dbname, index), obj);
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
                await DatabaseService.RenameIndex(new IndexModel(dbname, index), name);
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
                await DatabaseService.Update(new IndexModel(dbname, index), obj, id);
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
                await DatabaseService.DeleteDatabase(dbname);
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
                await DatabaseService.DeleteIndex(new IndexModel(dbname, index));
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
                var model = await DatabaseService.FindById(indexModel, id);
                await DatabaseService.Delete(indexModel, model);
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