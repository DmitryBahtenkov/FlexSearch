using Newtonsoft.Json.Linq;

namespace Analyzer.Models
{
    public class DocumentModel
    {
        /// <summary>
        /// Id для доступа к документам
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Тело документа
        /// </summary>
        public JObject Value { get; set; }
    }
}