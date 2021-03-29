using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace FlexSearch.Panel.Models.CoreModels
{
    public record DocumentModel
    {
        public Guid Id { get; set; }
        [DataType(DataType.MultilineText)]
        public JObject Value { get; set; }
    }
}