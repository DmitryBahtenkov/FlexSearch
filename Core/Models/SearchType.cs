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
        Match,
        /// <summary>
        /// Поиск по регулярному выражению
        /// </summary>
        Regex,
        /// <summary>
        /// Поиск по всему документу
        /// </summary>
        Full,
        /// <summary>
        /// Операция "или"
        /// </summary>
        Or,
        /// <summary>
        /// Операция "или не"
        /// </summary>
        NotOr,
        /// <summary>
        /// Операция "и не"
        /// </summary>
        NotAnd
    }
}