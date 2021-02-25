using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Configuration;
using Core.Exceptions;
using Core.Models;

namespace SearchApi.Services
{
    public static class ConfigurationService
    {
        private static readonly ConfigurationRepository _configurationRepository;

        static ConfigurationService()
        {
            _configurationRepository = new ConfigurationRepository();
        }

        public static async Task SetDefault()
        {
            var model = new ConfigurationModel
            {
                Host = "http://localhost",
                Port = 5000,
                Root = new RootUserModel {Password = "1234"},
                Users = new List<UserModel>(),
                Filters = new List<string>
                {
                    "LowerCase",
                    "Punctuation",
                    "Stemmer"
                },
                SyncHosts = new List<string>()
            };
            await _configurationRepository.SetConfig(model);
        }

        public static async Task Set(ConfigurationModel configurationModel)
        {
            var validationResult = ValidateConfig(configurationModel);
            if (!string.IsNullOrEmpty(validationResult))
            {
                throw new ConfigurationException(validationResult);
            }

            await _configurationRepository.SetConfig(configurationModel);
        }

        public static async Task<ConfigurationModel> Get()
        {
            if (!File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}config/config.conf"))
                await SetDefault();
            return await _configurationRepository.GetConfig();
        }

        public static async Task<List<UserModel>> GetUsers()
        {
            var config = await Get();
            var users = config.Users;
            users.Add(new UserModel
            {
                UserName = "root",
                Database = "all",
                Password = config.Root.Password
            });

            return users;
        }

        public static async Task CreateUser(UserModel userModel)
        {
            var config = await Get();
            if (config.Users.Contains(userModel) || userModel.UserName == "root")
                throw new ExistingUserException();
            config.Users.Add(userModel);
            await Set(config);
        }

        public static async Task UpdateUser(string userName, UserModel userModel)
        {
            var config = await Get();
            var user = config.Users.FirstOrDefault(x => x.UserName == userName);
            config.Users.Remove(user);
            config.Users.Add(userModel);
            await Set(config);
        }

        private static string ValidateConfig(ConfigurationModel configurationModel)
        {
            var validationContext = new ValidationContext(configurationModel);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(configurationModel, validationContext, validationResults, true))
            {
                return string.Join(',', validationResults);
            }

            return string.Empty;
        }
    }
}