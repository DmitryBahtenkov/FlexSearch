using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Analyzer.Models;
using Newtonsoft.Json.Linq;

namespace Storage.FileSystem
{
    public class AddObjectToIndex
    {
        private readonly WriteJsonFileCommand _writeJsonFileCommand;
        private readonly GetIdsCommand _getIdsCommand;

        public AddObjectToIndex()
        {
            _writeJsonFileCommand = new WriteJsonFileCommand();
            _getIdsCommand = new GetIdsCommand();
        }

        public async Task Add(string dbName, string indexName, string obj)
        {
            var path = $"/home/dmitry/Projects/GreatSearchEngine/Storage/bin/Debug/netcoreapp3.1/data/{dbName}/{indexName}";
            var ids = await GetIdsCommand.GetIds(dbName, indexName);
            var id = 0;
            if (ids.Any())
            {
                id = ids.Count;
            }
            
            var doc = new DocumentModel
            {
                Id = id,
                Value = JObject.Parse(obj)
            };

            path += $"/{id}.json";
            
            var res = File.Create(path);
            res.Close();
            
            await WriteJsonFileCommand.WriteFile(path, doc);
        }
    }
}