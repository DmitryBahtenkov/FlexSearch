using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Core.Models
{
    public record SearchModel
    {
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        public SearchType Type { get; set; }
        public string Key { get; set; }
        public string Term { get; set; }
        
        [JsonPropertyName("Sort")]
        public Dictionary<string, int> SortDict { get; set; }

        [System.Text.Json.Serialization.JsonIgnore] 
        public KeyValuePair<string, int> Sort => SortDict.FirstOrDefault();
    }
}