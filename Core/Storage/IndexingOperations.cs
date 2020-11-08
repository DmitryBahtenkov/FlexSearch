using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Analyzer;
using Core.Enums;
using Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Storage
{
    public class IndexingOperations
    {
        private readonly Indexer _indexer;
        private readonly GetDocumentsCommand _getDocuments;
        private string Path { get; set; }

        public IndexingOperations()
        {
            _indexer = new Indexer(new Analyzer.Analyzer(new Tokenizer(), new Normalizer(Languages.English)));
            _getDocuments = new GetDocumentsCommand();
        }
        public async Task<Dictionary<string, List<int>>> GetIndexes(IndexModel indexModel, string key)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}/indexing/{key}.json";
            using (var sr = new StreamReader(path))
            {
                var raw = await sr.ReadToEndAsync();
                return JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(raw);
            }
        }

        public async Task SetIndexes(IndexModel indexModel)
        {
            Path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}/indexing/";
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);

            var docs = await _getDocuments.Get(indexModel);
            foreach (var doc in docs)
            {
                await CreateIndexes(doc.Value, docs);
            }
        }
        
        private async Task CreateIndexes(JObject obj, List<DocumentModel> docs)
        {
            foreach (var (k, v) in obj)
            {
                if (v.Type == JTokenType.String)
                {
                    var path = Path + $"{k}.json";
                    if (!File.Exists(path))
                        File.Create(path).Close();
                    var dict = await _indexer.AddDocuments(docs, k);
                    await WriteJsonFileCommand.WriteFile(path, dict);
                }
                else if(v.Type == JTokenType.Object)
                {
                    await CreateIndexes(v.ToObject<JObject>(), docs);
                }
            }
        }
    }
}