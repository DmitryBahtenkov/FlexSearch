using System.Threading.Tasks;
using FlexSearch.Router.Models;
using Microsoft.AspNetCore.Http;

namespace FlexSearch.Router.Services
{
    public interface IRedirectService
    {
        public Task<RedirectResult> RedirectAll(HttpRequest request);
        public Task<Result> RedirectToSlave(HttpRequest request);
    }
}