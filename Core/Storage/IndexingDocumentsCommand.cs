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
    public class IndexingDocumentsCommand : BaseCommand
    {
        private readonly Indexer _indexer;
        private readonly Analyzer.Analyzer _analyzer;
        private readonly GetDocumentsCommand _getDocumentsCommand;
        private readonly WriteJsonFileCommand _writeJsonFileCommand;
        private string Path { get; set; }

        public IndexingDocumentsCommand()
        {
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer(Languages.English));
            _indexer = new Indexer(_analyzer);
            _getDocumentsCommand = new GetDocumentsCommand();
            _writeJsonFileCommand = new WriteJsonFileCommand();
        }

        public async Task Indexing(string dbName, string idxName)
        {
            Path = $"{AppDomain.BaseDirectory}data/{dbName}/{idxName}/indexing/";
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }

            var docs = await _getDocumentsCommand.Get(dbName, idxName);
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
                    await WriteJsonFileCommand.WriteFile(path, JsonConvert.SerializeObject(dict));
                }
                else if(v.Type == JTokenType.Object)
                {
                   await CreateIndexes(v.ToObject<JObject>(), docs);
                }
            }
        }
        
        
    }
}