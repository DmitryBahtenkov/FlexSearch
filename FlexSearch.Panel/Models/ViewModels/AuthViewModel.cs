using System.ComponentModel.DataAnnotations;

namespace FlexSearch.Panel.Models.ViewModels
{
    public class AuthViewModel
    {
        [Required(ErrorMessage = "Не указан адрес сервера")]
        public string Host { get; set; }
        
        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }
         
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}