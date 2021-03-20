using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Core.Configuration;
using Core.Exceptions;
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

            var user = (await GetUsers()).FirstOrDefault(x=>x.UserName == userName) ?? new UserModel();
            
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
        public static async Task<List<UserModel>> GetUsers()
        {
            var config = await ConfigurationService.Get();
            var users = config.Users;
            users.Add(new UserModel
            {
                UserName = "root",
                Database = "all",
                Password = config.Root.Password
            });

            return users;
        }
        public static async Task<UserModel> GetUser(string user)
        {
            var config = await ConfigurationService.Get();
            if (user == "root")
            {
                return new UserModel
                {
                    UserName = "root",
                    Password = config.Root.Password,
                    Database = "all"
                };
            }

            return config.Users.FirstOrDefault(u => u.UserName == user);
        }
        
        public static async Task<IEnumerable<object>> GetUsersNoPassword()
        {
            var config = await ConfigurationService.Get();
            var users = config.Users;
            users.Add(new UserModel
            {
                UserName = "root",
                Database = "all",
                Password = config.Root.Password
            });

            return users.Select(x=>new {x.UserName, x.Database});
        }

        public static async Task CreateUser(UserModel user)
        {
            var config = await ConfigurationService.Get();
            if (config.Users.Select(x => x.UserName).Contains(user.UserName))
            {
                throw new ExistingUserException();
            }

            if (!TryValidateUser(user, out var errors))
            {
                throw new ValidationException(errors);
            }
            config.Users.Add(user);
            await ConfigurationService.Set(config);
        }
        
        public static async Task UpdateUser(string userName, UserModel user)
        {
            var config = await ConfigurationService.Get();
            if (!config.Users.Select(x => x.UserName).Contains(userName))
            {
                throw new UserNotFoundException();
            }

            config.Users.RemoveAll(x => x.UserName == userName);

            if (!TryValidateUser(user, out var errors))
            {
                throw new ValidationException(errors);
            }
            config.Users.Add(user);
            await ConfigurationService.Set(config);
        }
        
        public static async Task DeleteUser(string userName)
        {
            var config = await ConfigurationService.Get();
            config.Users.RemoveAll(x => x.UserName == userName);
            await ConfigurationService.Set(config);
        }

        private static bool TryValidateUser(UserModel user, out string errors)
        {
            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(user, 
                validationContext, 
                validationResults, true))
            {
                errors = string.Join(',', validationResults);
                return false;
            }

            errors = string.Empty;
            return true;
        }
    }
}