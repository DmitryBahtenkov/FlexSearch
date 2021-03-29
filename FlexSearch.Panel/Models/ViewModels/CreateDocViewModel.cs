using System.ComponentModel.DataAnnotations;
using FlexSearch.Panel.Helpers;

namespace FlexSearch.Panel.Models.ViewModels
{
    public record CreateDocViewModel
    {
        [Required(ErrorMessage = "Название индекса обязательно")]
        public string Index {get;set;}
        [Required(ErrorMessage = "Название баз данных обязательно")]
        public string DbName {get;set;}

        [DataType(DataType.MultilineText)]
        [JsonType(ErrorMessage = "Значение должно представлять из себя json")]
        public string Value {get;set;}
    }
}