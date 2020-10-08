using Newtonsoft.Json.Linq;

namespace Core.Models
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
        public string Value { get; set; }
    }
}