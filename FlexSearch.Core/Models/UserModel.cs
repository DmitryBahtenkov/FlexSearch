using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public record UserModel
    {
        [Required(ErrorMessage = "Отсутствует имя пользователя")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Отсутствует пароль")]
        public string Password { get; set; }
        public string Database { get; set; }
    }

    public record RootUserModel
    {
        [Required(ErrorMessage = "Пароль корневого пользователя обязателен")]
        public string Password { get; set; }
    }
}