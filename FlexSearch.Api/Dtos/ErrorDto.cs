using System.Text.Json.Serialization;

namespace SearchApi.Dtos
{
    public record ErrorDto()
    {
        [JsonPropertyName("Type")]
        public ErrorsType Type { get; set; }
        [JsonPropertyName("Message")]
        public string Message { get; init; }

        public ErrorDto(ErrorsType type, string message) : this()
        {
            Type = type;
            Message = message;
        }
        public static ErrorDto GetAuthError() => new ErrorDto(ErrorsType.AuthError, "Incorrect auth data");
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ErrorsType
    {
        SyntaxError,
        SystemError,
        ValidationError,
        AuthError
    }
}