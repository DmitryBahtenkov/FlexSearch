using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Analyzer;
using Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Storage
{
    public class IndexingOperations
    {
        private readonly Indexer _indexer;
        private readonly GetOperations _getDocuments;
        private string Path { get; set; }

        public IndexingOperations()
        {
            _indexer = new Indexer(new Analyzer.Analyzer(new Tokenizer(), new Normalizer()));
            _getDocuments = new GetOperations();
        }
        public async Task<Dictionary<string, List<Guid>>> GetIndexesOneKey(IndexModel indexModel, string key)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}/indexing/{key}.json";
            using (var sr = new StreamReader(path))
            {
                var raw = await sr.ReadToEndAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, List<Guid>>>(raw);
            }
        }

        public async Task<List<Dictionary<string, List<Guid>>>> GetIndexesAllKeys(IndexModel indexModel, string key)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}/indexing/";
            var files = Directory.GetFiles(path);
            var result = new List<Dictionary<string, List<Guid>>>();
            foreach (var file in files.Where(x=>x.Contains(key)))
            {
                using (var sr = new StreamReader(file))
                {
                    var raw = await sr.ReadToEndAsync();
                    result.Add( JsonConvert.DeserializeObject<Dictionary<string, List<Guid>>>(raw));
                }
            }
            return result;
        }

        public async Task SetIndexes(IndexModel indexModel)
        {
            Path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}/indexing/";
            await FileOperations.CheckOrCreateDirectory(Path);

            var docs = await _getDocuments.GetDocuments(indexModel);
            foreach (var doc in docs)
            {
                await CreateIndexes(doc.Value, docs);
            }
        }
        
        private async Task CreateIndexes(JObject obj, List<DocumentModel> docs, params string[] keys)
        {
            foreach (var (k, v) in obj)
            {
                var newKeys = keys.Append(k).ToArray();
                await CheckJson( v, docs, newKeys);
            }
        }

        private async Task CheckJson(JToken v, List<DocumentModel> docs, params  string[] keys)
        {
            if (v.Type == JTokenType.String)
            {
                var path = keys.Aggregate(Path, (current, k) => current + $"{k}.");

                path += "json";
                if (!File.Exists(path))
                    File.Create(path).Close();
                var dict = await _indexer.AddDocuments(docs, keys);
                await FileOperations.WriteObjectToFile(path, dict);
            }
            else if(v.Type == JTokenType.Object)
            {
                var o = v.ToObject<JObject>();
                await CreateIndexes(o, docs, keys);
            }
            else if(v.Type == JTokenType.Array)
            {
                
            }
            else
            {
                var path = keys.Aggregate(Path, (current, k) => current + $"{k}.");

                path += "json";
                if (!File.Exists(path))
                    File.Create(path).Close();
                var dict = await _indexer.AddDocuments(docs, keys);
                await FileOperations.WriteObjectToFile(path, dict);
            }
        }
    }
}