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
            var ids = await GetIds(indexModel);
            var id = 0;
            
            if (ids.Any())
                id = ids.Count;

            var raw = obj.ToString();
            var doc = new DocumentModel
            {
                Id = id,
                Value = JObject.Parse(raw)
            };

            path += $"/{id}.json";
            await FileOperations.WriteObjectToFile(path, doc);
        }
        private Task<List<int>> GetIds(IndexModel indexModel)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}";
            var files = Directory.GetFiles(path);
            var result = 
                files
                    .Select(file =>
                    {
                        file = file.Replace('\\', '/');
                        return file.Split('/')[^1];
                    }).Select(str => str.Split('.')[0])
                    .Select(s=>Convert.ToInt32(s)).OrderBy(x=>x).ToList();


            return Task.FromResult(result);
        }
    }
}