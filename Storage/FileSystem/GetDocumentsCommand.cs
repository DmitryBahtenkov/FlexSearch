using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Analyzer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Storage.FileSystem
{
    public class GetDocumentsCommand
    {
        private readonly ReadJsonFileCommand _readJsonFileCommand;

        public GetDocumentsCommand()
        {
            _readJsonFileCommand = new ReadJsonFileCommand();
        }

        public Task<List<DocumentModel>> Get(string dbName, string indexName)
        {
            var path = $"/home/dmitry/Projects/GreatSearchEngine/Storage/bin/Debug/netcoreapp3.1/data/{dbName}/{indexName}";
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            var docs = Directory.GetFiles(path);
            var result = docs.Select(doc => JsonConvert.DeserializeObject<DocumentModel>(File.ReadAllText(doc))).ToList();

                
            return Task.FromResult(result);
        }
    }


}