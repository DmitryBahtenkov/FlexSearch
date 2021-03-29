using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;

namespace FlexSearch.Router
{
    public static class Extensions
    {
        public static HttpRequestMessage GetMessage(this HttpRequest request)
        {
            var hreqmf = new HttpRequestMessageFeature(request.HttpContext);
            return hreqmf.HttpRequestMessage;
        }
    }
}