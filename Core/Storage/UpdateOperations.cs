using System;
using System.Threading.Tasks;
using Core.Models;
using Newtonsoft.Json.Linq;

namespace Core.Storage
{
    public class UpdateOperations
    {
        public async Task RenameIndex(IndexModel indexModel, string newName)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}";
            await FileOperations.RenameDirectory(path, newName);
        }
        
        public async Task RenameDatabase(IndexModel indexModel, string newName)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel.DatabaseName}";
            await FileOperations.RenameDirectory(path, newName);
        }

        public async Task UpdateObject(IndexModel indexModel, long id, object newValue)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}/{id}.json";
            var current = await FileOperations.ReadObjectFromFile<DocumentModel>(path);
            var raw = newValue.ToString();
            current.Value = JObject.Parse(raw);
            await FileOperations.WriteObjectToFile(path, current);
        }
    }
}