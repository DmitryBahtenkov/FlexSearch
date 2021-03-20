using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SearchApi.Dtos
{
    public record DocumentDto
    {
        [JsonPropertyName("Id")]
        public  Guid Id { get; set; }
        [JsonPropertyName("Value")]
        public JsonDocument Value { get; set; }
    }
}