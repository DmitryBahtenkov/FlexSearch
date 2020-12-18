using System.Threading.Tasks;
using Core.Models;
using Core.Users;
using Microsoft.AspNetCore.Http;

namespace SearchApi.Services
{
    public class UserService
    {
        private const string NameKey = "User";
        private const string PasswordKey = "Password";
        
        public UserRepository UserRepository { get; set; }

        public UserService()
        {
            UserRepository ??= new UserRepository();
        }

        public async Task<UserModel> CheckAuthorize(HttpRequest request, bool root = false)
        {
            if (request.Headers.ContainsKey(NameKey) && request.Headers.ContainsKey(PasswordKey))
            {
                var userName = request.Headers[NameKey].ToString();
                var pass = request.Headers[PasswordKey].ToString();

                var user = await UserRepository.GetUser(userName);
                if (root)
                {
                    if (user.UserName != "root")
                        return null;
                }
                if (pass == user.Password)
                    return user;
            }

            return null;
        }
    }
}