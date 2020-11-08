using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Newtonsoft.Json;

namespace Core.Storage
{
    public class GetDocumentsCommand
    {

        public Task<List<DocumentModel>> Get(string dbName, string indexName)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{dbName}/{indexName}";
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            var docs = Directory.GetFiles(path);
            var result = docs.Select(doc => 
                JsonConvert.DeserializeObject<DocumentModel>(File.ReadAllText(doc))).ToList();
            
            return Task.FromResult(result);
        }
    }


}