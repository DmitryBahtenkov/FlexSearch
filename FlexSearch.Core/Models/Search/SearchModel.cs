using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Core.Models.Search
{
    public record SearchModel : BaseSortingModel
    {
        /// <summary>
        /// Тип поиска
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SearchType Type { get; set; }
        /// <summary>
        /// Ключ, по которому нужно производить поиск
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Текст поискового запроса
        /// </summary>
        public string Term { get; set; }
    }
}