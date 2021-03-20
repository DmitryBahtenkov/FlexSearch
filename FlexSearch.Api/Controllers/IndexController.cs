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
using SearchApi.Dtos;
using SearchApi.Dtos.Mappings;
using SearchApi.Services;


namespace SearchApi.Controllers
{
    [Route("")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly ILogger<IndexController> _logger;

        public IndexController(
            ILogger<IndexController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetDatabases()
        {
            if (await UserService.CheckAuthorize(Request) is null)
                return Unauthorized(ErrorDto.GetAuthError());
            return Ok(await DatabaseService.GetDatabases());
        }
        
        [HttpGet("index/{dbname}")]
        public Task<IActionResult> GetIndexes(string dbname)
        {
            throw new NotImplementedException("Не реализовано!");
        }
        
        [HttpGet("index/{dbname}/{index}/")]
        public async Task<IActionResult> GetDocuments(string dbname, string index)
        {
            return Ok(DocumentMapper.MapToDtos(await DatabaseService.GetAll(new IndexModel(dbname, index))));
        }
        
        [HttpGet("index/{dbname}/{index}/{id}")]
        public async Task<IActionResult> GetDocument(string dbname, string index, string id)
        {
            if (await UserService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized(ErrorDto.GetAuthError());
            if (Guid.TryParse(id, out var guid))
            {
                var result = await DatabaseService.FindById(new IndexModel(dbname, index), guid);
                if (result is null)
                    return NoContent();
                return Ok(DocumentMapper.MapToDto(result));
            }

            return BadRequest(new ErrorDto(ErrorsType.SyntaxError, $"{id} is not GUID"));
        }

        [HttpPost("index/{dbname}/{index}/")]
        public async Task<IActionResult> CreateIndex([FromBody] object obj, string dbname, string index)
        {
            if (await UserService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized(ErrorDto.GetAuthError());
            try
            {
                var id = await DatabaseService.Insert(new IndexModel(dbname, index), obj);
                return StatusCode(201, id);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: Create object {obj} in {dbname}/{index}, Error: {ex}");
                return StatusCode(500, new ErrorDto(ErrorsType.SystemError, ex.Message));
            }
        }

        [HttpPut("index/{dbname}/{index}/rename")]
        public async Task<IActionResult> RenameIndex(string dbname, string index, string name)
        {
            if (await UserService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized(ErrorDto.GetAuthError());
            try
            {
                await DatabaseService.RenameIndex(new IndexModel(dbname, index), name);
                _logger.Log(LogLevel.Information, $"INFO: Rename index {dbname}/{index}. New name: {name}");
                return StatusCode(202);
            }
            catch (FileNotFoundException)
            {
                _logger.Log(LogLevel.Error, $"ERROR: index {index} not found");
                return BadRequest(new ErrorDto(ErrorsType.SyntaxError, $"Index {index} not found"));
            }
            catch (IOException)
            {
                _logger.Log(LogLevel.Error, $"ERROR: index {index} not found");
                return BadRequest(new ErrorDto(ErrorsType.SyntaxError, $"Index {index} already exists"));
            }
        }
        
        [HttpPut("index/{dbname}/{index}/{id}/")]
        public async Task<IActionResult> UpdateObject([FromBody] object obj,string dbname, string index, string id)
        {
            if (await UserService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized(ErrorDto.GetAuthError());
            try
            {
                await DatabaseService.Update(new IndexModel(dbname, index), obj, id);
                _logger.Log(LogLevel.Information, $"INFO: Update object with id: {id} in {dbname}/{index}. New object: {obj}");
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: error updating with id: {id}, exception: {ex}");
                return BadRequest(new ErrorDto(ErrorsType.SystemError, $"{ex.Message}"));
            }
            return StatusCode(202);

        }
        
        [HttpDelete("index/{dbname}/")]
        public async Task<IActionResult> DeleteDatabase(string dbname)
        {
            if (await UserService.CheckAuthorize(Request, true) is null)
                return Unauthorized(ErrorDto.GetAuthError());
            try
            {
                await DatabaseService.DeleteDatabase(dbname);
                _logger.Log(LogLevel.Information, $"INFO: Delete database {dbname}");
            }
            catch (DirectoryNotFoundException)
            {
                _logger.Log(LogLevel.Error, $"ERROR:  Database {dbname} not found");
                return NoContent();
            }

            return Ok();
        }
        
        [HttpDelete("index/{dbname}/{index}/")]
        public async Task<IActionResult> DeleteIndex(string dbname, string index)
        {
            if (await UserService.CheckAuthorize(Request, true) is null)
                return Unauthorized(ErrorDto.GetAuthError());
            try
            {
                await DatabaseService.DeleteIndex(new IndexModel(dbname, index));
                _logger.Log(LogLevel.Information, $"INFO: Delete index {dbname}/{index}");
            }
            catch (FileNotFoundException)
            {
                _logger.Log(LogLevel.Error, $"ERROR: index {dbname}/{index} not found");
                return NoContent();
            }

            return Ok();
        }

        [HttpDelete("index/{dbname}/{index}/{id}/")]
        public async Task<IActionResult> DeleteObject(string dbname, string index, string id)
        {
            if (await UserService.CheckAuthorize(Request, true) is null)
                return Unauthorized();
            if (Guid.TryParse(id, out var guid))
            {
                try
                {
                    _logger.Log(LogLevel.Information, $"INFO: Delete object from {dbname}/{index} with id: {id}");
                    var indexModel = new IndexModel(dbname, index);
                    var model = await DatabaseService.FindById(indexModel, guid);
                    await DatabaseService.Delete(indexModel, model);
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, $"ERROR: deleting document with id: {id}. Exception: {ex}");
                    return BadRequest(new ErrorDto(ErrorsType.SystemError, ex.Message));
                }
            }

            return BadRequest(new ErrorDto(ErrorsType.SyntaxError, $"{id} is not GUID"));
        }
    }
}