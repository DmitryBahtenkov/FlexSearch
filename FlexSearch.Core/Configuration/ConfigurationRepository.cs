using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Newtonsoft.Json;

namespace Core.Configuration
{
    public class ConfigurationRepository
    {
        private readonly string _path = $"{AppDomain.CurrentDomain.BaseDirectory}config";
        
        public async Task SetConfig(ConfigurationModel configurationModel)
        {
            await FileOperations.CheckOrCreateDirectory(_path);
            using (var stream = new StreamWriter(_path + "/config.conf"))
            {
                await stream.WriteAsync(JsonConvert.SerializeObject(configurationModel));
            }
        }

        public async Task<ConfigurationModel> GetConfig()
        {
            using (var stream = new StreamReader(_path + "/config.conf"))
            {
                var raw = await stream.ReadToEndAsync();
                return JsonConvert.DeserializeObject<ConfigurationModel>(raw);
            }
        }
    }
}