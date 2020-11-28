using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Newtonsoft.Json.Linq;

namespace Core.Analyzer
{
    public class Indexer
    {
        private Dictionary<string, List<long>> _indexCollection;
        private readonly Analyzer _analyzer;

        public Indexer(Analyzer analyzer)
        {
            _analyzer = analyzer;
        }


        public async Task<Dictionary<string, List<long>>> AddDocuments(IList<DocumentModel> documents, params string[] keys)
        {
            _indexCollection = new Dictionary<string, List<long>>();

            foreach (var document in documents)
            {
                JToken obj = document.Value;
                if (keys.Length > 0)
                    obj = keys.Aggregate(obj, (current, k) => current?[k]);
                    
                if(obj is null) continue;
                foreach (var str in await _analyzer.Anal(obj.ToString()))
                {
                    if (_indexCollection.ContainsKey(str))
                    {
                        if(_indexCollection[str].Contains(document.Id))
                            continue;
                        _indexCollection[str].Add(document.Id);
                    }
                    else
                    {
                        _indexCollection.Add(str, new List<long> {document.Id});
                    }
                }
            }
            
            return _indexCollection;
        }
    }
}