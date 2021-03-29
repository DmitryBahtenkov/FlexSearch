using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FlexSearch.Panel.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlexSearch.Panel.Models.ViewModels
{
    public record DocumentViewModel
    {
        public Guid? Id { get; set; }

        [DataType(DataType.MultilineText)]
        [JsonType(ErrorMessage = "Текст должен представлять из себя JSON")]
        public string Value { get; set; }

        public JObject Json
        {
            get
            {
                try
                {
                    return JsonConvert.DeserializeObject<JObject>(Value);
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
        
        public string DbName { get; set; }
        public string Index { get; set; }
    }
}