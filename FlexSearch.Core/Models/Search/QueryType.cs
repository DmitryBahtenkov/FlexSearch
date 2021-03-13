using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Models.Search
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum QueryType
    {
        Or,
        And
    }
}