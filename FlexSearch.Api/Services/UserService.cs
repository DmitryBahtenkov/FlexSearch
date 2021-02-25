using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SearchApi.Services
{
    public static class UserService
    {
        private const string NameKey = "User";
        private const string PasswordKey = "Password";

        private static readonly ConfigurationModel _configuration;
        

        static UserService()
        {
            _configuration = ConfigurationService.Get().GetAwaiter().GetResult();
        }

        public static async Task<UserModel> CheckAuthorize(HttpRequest request, bool root = false, string database = null)
        {
            if (!request.Headers.ContainsKey(NameKey) || !request.Headers.ContainsKey(PasswordKey)) 
                return null;
            var userName = request.Headers[NameKey].ToString();
            var pass = request.Headers[PasswordKey].ToString();

            var user = (await ConfigurationService.GetUsers()).FirstOrDefault(x=>x.UserName == userName) ?? new UserModel();
            
            if (root)
            {
                if (user.UserName != "root")
                    return null;
            }

            if (pass != user.Password) 
                return null;
            if (database is null) 
                return user;
            if (user.Database == database || user.Database == "all")
                return user;
            return null;

        }
    }
}