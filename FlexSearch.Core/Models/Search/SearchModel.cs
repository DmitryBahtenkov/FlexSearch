using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Core.Models.Search
{
    public record SearchModel : BaseSortingModel
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SearchType Type { get; set; }
        public string Key { get; set; }
        public string Term { get; set; }
    }
}