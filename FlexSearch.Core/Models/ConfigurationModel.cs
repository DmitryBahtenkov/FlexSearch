﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Core.Models
{
    public record ConfigurationModel
    {
        /// <summary>
        /// Адрес поисковой системы
        /// </summary>
        [Required(ErrorMessage = "Host is required")]
        [Url(ErrorMessage = "Host is url-type")]
        public string Host { get; set; }
        /// <summary>
        /// Порт поисковой системы
        /// </summary>
        [Required(ErrorMessage = "Port is required")]
        public int Port { get; set; }
        /// <summary>
        /// Данные о рут-пользователе
        /// </summary>
        [Required(ErrorMessage = "Root-password is required")]
        public RootUserModel Root { get; set; }
        /// <summary>
        /// Данные об остальных пользователях
        /// </summary>
        public List<UserModel> Users { get; set; }
        /// <summary>
        /// Список фильтров
        /// </summary>
        public List<string> Filters { get; set; }
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