using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Core.Configuration;
using Core.Exceptions;
using Core.Models;

namespace SearchApi.Services
{
    public class ConfigurationService
    {
        private readonly ConfigurationRepository _configurationRepository;

        public ConfigurationService()
        {
            _configurationRepository = new ConfigurationRepository();
        }

        public async Task SetDefault()
        {
            var model = new ConfigurationModel
            {
                Host = "http://localhost",
                Port = 5000,
                Root = new RootUserModel {Password = "1234"},
                Users = new List<UserModel>(),
                FiltersNames = new List<string>
                {
                    "LowerCase",
                    "Punctuation",
                    "Stemmer"
                },
                SyncHosts = new List<string>()
            };
            await _configurationRepository.SetConfig(model);
        }

        public async Task Set(ConfigurationModel configurationModel)
        {
            var validationResult = ValidateConfig(configurationModel);
            if (!string.IsNullOrEmpty(validationResult))
            {
                throw new ConfigurationException(validationResult);
            }

            await _configurationRepository.SetConfig(configurationModel);
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