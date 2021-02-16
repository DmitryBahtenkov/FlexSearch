using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SearchApi.Services
{
    public class UserService
    {
        private const string NameKey = "User";
        private const string PasswordKey = "Password";
        
        public UserRepository UserRepository { get; set; }
        private readonly ILogger<UserService> _logger;

        public UserService()
        {
            UserRepository ??= new UserRepository();
            _logger = LoggerFactory
                .Create(x => x.AddFile($"{AppDomain.CurrentDomain.BaseDirectory}Logs/" + "{Date}.log"))
                .CreateLogger<UserService>();
        }

        public async Task<UserModel> CheckAuthorize(HttpRequest request, bool root = false, string database = null)
        {
            if (request.Headers.ContainsKey(NameKey) && request.Headers.ContainsKey(PasswordKey))
            {
                var userName = request.Headers[NameKey].ToString();
                var pass = request.Headers[PasswordKey].ToString();
                _logger.Log(LogLevel.Information, $"User {userName} trying authorize");
                var user = await UserRepository.GetUser(userName);
                if (root)
                {
                    if (user.UserName != "root")
                        return null;
                }

                if (pass == user.Password)
                {
                    if (database is not null)
                    {
                        if (user.Database == database || user.Database == "all")
                            return user;
                        return null;
                    }

                    return user;
                }
            }

            return null;
        }
    }
}