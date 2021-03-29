using System;
using System.Net.Http;
using System.Threading.Tasks;
using FlexSearch.Router.Models;
using FlexSearch.Router.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FlexSearch.Router.Controllers
{
    [Route("{url}")]
    public class MainController : ControllerBase
    {
        private readonly IRedirectService _redirectService;
        private readonly HttpClient _httpClient;

        public MainController(
            ISettingsService settingsService, 
            HttpClient httpClient, IRedirectService redirectService)
        {
            _httpClient = httpClient;
            _redirectService = redirectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRequest()
        {
            var result = await _redirectService.RedirectToSlave(Request);
            var content = await result.ResponseMessage.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
                return StatusCode((int) result.ResponseMessage.StatusCode);
            else
                return StatusCode((int) result.ResponseMessage.StatusCode, JsonConvert.DeserializeObject(content));
        }

        [HttpPost]
        public async Task<IActionResult> PostRequest()
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public async Task<IActionResult> PutRequest()
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteRequest()
        {
            throw new NotImplementedException();
        }
        
    }
}