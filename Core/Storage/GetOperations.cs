using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Newtonsoft.Json;

namespace Core.Storage
{
    public class GetOperations
    {
        public Task<List<DocumentModel>> GetDocuments(IndexModel indexModel)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}";
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            var docs = Directory.GetFiles(path);
            var result = docs.Select(doc => 
                JsonConvert.DeserializeObject<DocumentModel>(File.ReadAllText(doc))).ToList();
            
            return Task.FromResult(result);
        }

        public Task<List<string>> GetDatabases()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/";
            FileOperations.CheckOrCreateDirectory(path);
            var databases = Directory.GetDirectories(path).ToList();
            return Task.FromResult(databases);
        }

        public Task<List<string>> GetIndexes(string dbName)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{dbName}";
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            
            return Task.FromResult(Directory.GetDirectories(path).ToList());
        }
    }


}