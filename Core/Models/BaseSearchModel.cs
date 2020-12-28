using System.Text.Json.Serialization;

namespace Core.Models
{
    public record BaseSearchModel
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SearchType Type { get; set; }
        public string Key { get; set; }
        public string Term { get; set; }
    }
}