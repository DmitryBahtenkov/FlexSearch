using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SearchType
    {
        /// <summary>
        /// Полнотекстовый поиск
        /// </summary>
        Fulltext,
        /// <summary>
        /// Полнотекстовый поиск с ошибками
        /// </summary>
        Errors,
        /// <summary>
        /// Поиск на точное совпадение
        /// </summary>
        Match
    }
}