using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Models
{
    public record DocumentModel
    {
        /// <summary>
        /// Id для доступа к документам
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Тело документа
        /// </summary>
        public JObject Value { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this).Replace("\"", "");
        }
    }
}