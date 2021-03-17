using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public record UserModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public string Database { get; set; }
    }

    public record RootUserModel
    {
        [Required(ErrorMessage = "Root password is required")]
        public string Password { get; set; }
    }
}