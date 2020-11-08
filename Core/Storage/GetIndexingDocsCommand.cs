using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Storage
{
    public class GetIndexingDocsCommand
    {
        //todo:добавить тесты
        public async Task<Dictionary<string, List<int>>> Get(string dbName, string idxName, string key)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{dbName}/{idxName}/indexing/{key}.json";
            using var sr = new StreamReader(path);
            var raw = await sr.ReadToEndAsync();
            return JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(raw);
        }
    }
}