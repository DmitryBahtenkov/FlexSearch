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

        private List<KeyValuePair<string, Dictionary<string, Guid>>> Dictionary { get; set; }
        private string Path { get; set; }

        public IndexingOperations()
        {
            _indexer = new Indexer(new Analyzer.Analyzer(new Tokenizer(), new Normalizer()));
            Dictionary = new List<KeyValuePair<string, Dictionary<string, Guid>>>();
        }

        
        
        public async Task<List<KeyValuePair<string, Dictionary<string, Guid>>>> CreateIndexes(JObject obj, DocumentModel documentModel, params string[] keys)
        {
            foreach (var (k, v) in obj)
            {
                var newKeys = keys.Append(k).ToArray();
                await CheckJson( v, documentModel, newKeys);
            }
            return Dictionary;
        }
        
        
        private async Task CheckJson(JToken v,DocumentModel model , params  string[] keys)
        {
            if (v.Type == JTokenType.String)
            {
                var path = keys.Aggregate(Path, (current, k) => current + $"{k}.");
                
                var dict = await _indexer.AddDocuments(model, 0, keys);
                Dictionary.Add(new KeyValuePair<string, Dictionary<string, Guid>>(path, dict));
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

                    path += $"{i}";
                    var dict = await _indexer.AddDocuments(model, i, keys);
                    Dictionary.Add(new KeyValuePair<string, Dictionary<string, Guid>>(path, dict));
                }
            }
            else
            {
                var path = keys.Aggregate(Path, (current, k) => current + $"{k}");
                
                var dict = await _indexer.AddDocuments(model, 0, keys);
                Dictionary.Add(new KeyValuePair<string, Dictionary<string, Guid>>(path, dict));
            }
        }
    }
}