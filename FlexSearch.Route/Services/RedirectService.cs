using System.Threading.Tasks;
using FlexSearch.Router.Models;
using Microsoft.AspNetCore.Http;

namespace FlexSearch.Router.Services
{
    public class RedirectService : IRedirectService
    {
        private readonly ISettingsService _settingsService;

        public RedirectService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public Task<RedirectResult> RedirectAll(HttpRequest request)
        {
            throw new System.NotImplementedException();
        }

        public Task<Result> RedirectToSlave(HttpRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}