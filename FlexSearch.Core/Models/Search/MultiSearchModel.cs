using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Core.Models.Search
{
    public record MultiSearchModel : BaseSortingModel
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public QueryType QueryType { get; set; }
        public SearchModel[] Searches { get; set; }
    }
}