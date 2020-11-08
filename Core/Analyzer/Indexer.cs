using System;
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
        private readonly Tokenizer _tokenizer;

        public Indexer(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }


        public async Task<Dictionary<string, List<long>>> AddDocuments(IList<DocumentModel> documents, string key, string lang = "en")
        {
            _indexCollection = new Dictionary<string, List<long>>();
            foreach (var document in documents)
            {
                if(document == null) continue;
                foreach (var str in await _tokenizer.Tokenize(document.Value[key]?.ToString(), lang))
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