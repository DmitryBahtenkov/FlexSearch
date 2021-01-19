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
        private Dictionary<string, Dictionary<string, Guid>> Dictionary { get; set; }
        private string Path { get; set; }

        public IndexingOperations()
        {
            _indexer = new Indexer(new Analyzer.Analyzer(new Tokenizer(), new Normalizer()));
            _getDocuments = new GetOperations();
            Dictionary = new Dictionary<string, Dictionary<string, Guid>>();
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

        public async Task<Dictionary<string, Dictionary<string, List<Guid>>>> SetIndexes(IndexModel indexModel)
        {
            Path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}/indexing/";
            await FileOperations.CheckOrCreateDirectory(Path);

            var docs = await _getDocuments.GetDocuments(indexModel);
            foreach (var doc in docs)
            {
                await CreateIndexes(doc.Value, docs);
            }
            return null;
        }
        
        public async Task<Dictionary<string, Dictionary<string, List<Guid>>>> CreateIndexes(JObject obj, List<DocumentModel> docs, params string[] keys)
        {
            foreach (var (k, v) in obj)
            {
                var newKeys = keys.Append(k).ToArray();
                await CheckJson( v, docs, newKeys);
            }
            return null;
        }
        
        public async Task<Dictionary<string, Dictionary<string, Guid>>> CreateIndexes(JObject obj, DocumentModel documentModel, params string[] keys)
        {
            foreach (var (k, v) in obj)
            {
                var newKeys = keys.Append(k).ToArray();
                await CheckJson( v, documentModel, newKeys);
            }
            return Dictionary;
        }

        private async Task CheckJson(JToken v, List<DocumentModel> docs, params  string[] keys)
        {
            if (v.Type == JTokenType.String)
            {
                var path = keys.Aggregate(Path, (current, k) => current + $"{k}.");

                path += "pidx";
                if (!File.Exists(path))
                    File.Create(path).Close();
                var dict = await _indexer.AddDocuments(docs, 0, keys);

            }
            else if(v.Type == JTokenType.Object)
            {
                var o = v.ToObject<JObject>();
                await CreateIndexes(o, docs, keys);
            }
            else if(v.Type == JTokenType.Array)
            {
                var tmp = (JArray) v;
                for (var i = 0; i < tmp.Count; i++)
                {
                    var path = Path;
                    foreach (var key in keys)
                    {
                        path += key + ".";
                    }

                    path += $"{i}.pidx";
                    if (!File.Exists(path))
                        File.Create(path).Close();
                    var dict = await _indexer.AddDocuments(docs, i, keys);

                }
            }
            else
            {
                var path = keys.Aggregate(Path, (current, k) => current + $"{k}.");

                path += "pidx";
                if (!File.Exists(path))
                    File.Create(path).Close();
                var dict = await _indexer.AddDocuments(docs, 0, keys);

            }
        }
        
        private async Task CheckJson(JToken v,DocumentModel model , params  string[] keys)
        {
            if (v.Type == JTokenType.String)
            {
                var path = keys.Aggregate(Path, (current, k) => current + $"{k}.");

                path += "pidx";
                if (!File.Exists(path))
                    File.Create(path).Close();
                var dict = await _indexer.AddDocuments(model, 0, keys);
                Dictionary.Add(path, dict);
            }
            else if(v.Type == JTokenType.Object)
            {
                var o = v.ToObject<JObject>();
                await CreateIndexes(o, model, keys);
            }
            else if(v.Type == JTokenType.Array)
            {
                var tmp = (JArray) v;
                for (var i = 0; i < tmp.Count; i++)
                {
                    var path = Path;
                    foreach (var key in keys)
                    {
                        path += key + ".";
                    }

                    path += $"{i}.pidx";
                    if (!File.Exists(path))
                        File.Create(path).Close();
                    var dict = await _indexer.AddDocuments(model, i, keys);
                    Dictionary.Add(path, dict);
                }
            }
            else
            {
                var path = keys.Aggregate(Path, (current, k) => current + $"{k}.");

                path += "pidx";
                if (!File.Exists(path))
                    File.Create(path).Close();
                var dict = await _indexer.AddDocuments(model, 0, keys);
                Dictionary.Add(path, dict);
            }
        }
    }
}