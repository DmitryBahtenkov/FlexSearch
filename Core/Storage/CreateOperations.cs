using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Newtonsoft.Json.Linq;

namespace Core.Storage
{
    public class CreateOperations
    {
        public async Task CreateIndexAndAddObject(IndexModel indexModel, object obj)
        {
            await CreateIndex(indexModel);
            await AddObject(indexModel, obj);
        }
        public Task CreateIndex(IndexModel indexModel)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}";
            FileOperations.CheckOrCreateDirectory(path);
            return Task.CompletedTask;
        }

        public async Task AddObject(IndexModel indexModel, object obj)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}";
            
            var raw = obj.ToString();
            var doc = new DocumentModel
            {
                Id = Guid.NewGuid(),
                Value = JObject.Parse(raw)
            };

            path += $"/{doc.Id}.json";
            await FileOperations.WriteObjectToFile(path, doc);
        }
    }
}