using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SearchApi.Dtos;
using SearchApi.Dtos.Mappings;
using SearchApi.Services;

namespace SearchApi.Controllers
{
    [ApiController]
    [Route("search")]
    public class SearchController : ControllerBase
    {
        private readonly SearcherService _searcher;
        private readonly ILogger<SearchController> _logger;

        public SearchController(SearcherService searcher, 
            ILogger<SearchController> logger)
        {
            _searcher = searcher;
            _logger = logger;
        }

        [HttpGet("/search/{dbname}/{index}")]
        public async Task<IActionResult> Search([FromBody] SearchModel searchModel, string dbname, string index)
        {
            if (await UserService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized(ErrorDto.GetAuthError());
            try
            {
                var docs = await _searcher.Search(new IndexModel(dbname, index), searchModel);
                var result = docs.Select(DocumentMapper.MapToDto);

                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: Search {searchModel.Type} - {searchModel.Key}:{searchModel.Term} in {dbname}/{index}, Error: {ex}");
                return BadRequest(new ErrorDto(ErrorsType.SystemError, ex.Message));
            }
        }
        
        [HttpGet("/multi-search/{dbname}/{index}")]
        public async Task<IActionResult> MultiSearch([FromBody] MultiSearchModel searchModel, string dbname, string index)
        {
            if (await UserService.CheckAuthorize(Request, false, dbname) is null)
                return Unauthorized(ErrorDto.GetAuthError());
            try
            {
                var docs = await _searcher.MultiSearch(new IndexModel(dbname, index), searchModel);
                var result = docs.Select(DocumentMapper.MapToDto);

                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.Log(LogLevel.Error, $"ERROR: Search {searchModel.QueryType} in {dbname}/{index}, Error: {ex}");
                return BadRequest(new ErrorDto(ErrorsType.SystemError, ex.Message));
            }
        }
    }
}