using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Core.Models
{
    public record ConfigurationModel
    {
        /// <summary>
        /// Адрес поисковой системы
        /// </summary>
        [Required(ErrorMessage = "Хост сервера обязателен!")]
        public string Host { get; set; }
        /// <summary>
        /// Порт поисковой системы
        /// </summary>
        [Required(ErrorMessage = "Порт сервера обязателен")]
        public int Port { get; set; }
        /// <summary>
        /// Данные о рут-пользователе
        /// </summary>
        public RootUserModel Root { get; set; }
        /// <summary>
        /// Данные об остальных пользователях
        /// </summary>
        public List<UserModel> Users { get; set; }
        /// <summary>
        /// Список фильтров
        /// </summary>
        public List<string> FiltersNames { get; set; }
        /// <summary>
        /// Список адресов других экземпляров системы для синхронизации
        /// </summary>
        public List<string> SyncHosts { get; set; }

        public Dictionary<string, object> ToDictionary()
        {
            var raw = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);
        }
    }
}