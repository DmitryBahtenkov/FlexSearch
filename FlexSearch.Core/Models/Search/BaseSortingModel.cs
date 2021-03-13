using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Core.Models.Search
{
    public record BaseSortingModel
    {
        [JsonPropertyName("Sort")]
        public Dictionary<string, int> SortDict { get; set; }

        [JsonIgnore] 
        public KeyValuePair<string, int> Sort => SortDict.FirstOrDefault();
    }
}