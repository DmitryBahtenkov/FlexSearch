using Microsoft.AspNetCore.Http;

namespace SearchApi.Services
{
    public class UserService
    {
        private const string NameKey = "User";
        private const string PasswordKey = "Password";

        public bool CheckAuthorize(HttpRequest request)
        {
            if (request.Headers.ContainsKey(NameKey) && request.Headers.ContainsKey(PasswordKey))
            {
                var userName = request.Headers[NameKey].ToString();
                var pass = request.Headers[PasswordKey].ToString();
                
                //todo: авторизация

                return true;
            }

            return false;
        }
    }
}