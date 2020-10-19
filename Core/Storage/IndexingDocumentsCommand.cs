using System.IO;
using System.Threading.Tasks;
using Core.Analyzer;
using Core.Enums;
using Newtonsoft.Json;

namespace Core.Storage
{
    public class IndexingDocumentsCommand : BaseCommand
    {
        private readonly Indexer _indexer;
        private readonly GetDocumentsCommand _getDocumentsCommand;

        public IndexingDocumentsCommand()
        {
            _indexer = new Indexer(new Analyzer.Analyzer(new Tokenizer(), new Normalizer(Languages.English)));
            _getDocumentsCommand = new GetDocumentsCommand();
        }

        public async Task Indexing(string dbName, string idxName)
        {
            var path = $"{AppDomain.BaseDirectory}data/{dbName}/{idxName}/indexing";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var docs = await _getDocumentsCommand.Get(dbName, idxName);
            var indexes = await _indexer.Add(docs);
            foreach (var pair in indexes)
            {
                path += $"/{pair.Key}.json";
                if (!File.Exists(path))
                    File.Create(path).Close();
                await WriteJsonFileCommand.WriteFile(path, JsonConvert.SerializeObject(pair));
                path = $"{AppDomain.BaseDirectory}data/{dbName}/{idxName}/indexing";
            }
            
        }
        
    }
}