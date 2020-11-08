using Newtonsoft.Json;
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
        public JObject Value { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}