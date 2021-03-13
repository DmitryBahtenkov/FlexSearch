using System.Text.Json.Serialization;

namespace Core.Models.Search
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum QueryType
    {
        Or,
        And
    }
}