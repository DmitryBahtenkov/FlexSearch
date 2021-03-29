using System.Collections.Generic;
using System.Net.Http;

namespace FlexSearch.Router.Models
{
    public record RedirectResult
    {
        public Result MasterResult { get; set; }
        public List<Result> SlaveResults { get; set; }
    }

    public record Result
    {
        public bool IsSuccess { get; set; }
        public string Url { get; set; }
        public string Error { get; set; }
        public HttpResponseMessage ResponseMessage { get; set; }
    }
}