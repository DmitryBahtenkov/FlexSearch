namespace Core.Models
{
    public record UserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
    }
}