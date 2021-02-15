#nullable enable
using Newtonsoft.Json.Linq;

namespace Core.Analyzer.Commands
{
    public static class JsonCommand
    {
        public static bool CheckIsString(JToken? token)
        {
            if (token is not null)
            {
                return token.Type == JTokenType.Boolean ||
                       token.Type == JTokenType.Integer ||
                       token.Type == JTokenType.Float ||
                       token.Type == JTokenType.String ||
                       token.Type == JTokenType.Property;
            }

            return false;
        }

        public static JToken? GetValueForKey(JToken? token, string key)
        {
            return token?.SelectToken($"$..{key}", false);
        }
    }
}