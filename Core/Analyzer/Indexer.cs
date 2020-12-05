using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Analyzer.Commands;
using Core.Models;
using Newtonsoft.Json.Linq;

namespace Core.Analyzer
{
    public class Indexer
    {
        private Dictionary<string, List<Guid>> _indexCollection;
        private readonly Analyzer _analyzer;

        public Indexer(Analyzer analyzer)
        {
            _analyzer = analyzer;
        }


        public async Task<Dictionary<string, List<Guid>>> AddDocuments(IList<DocumentModel> documents, params string[] keys)
        {
            _indexCollection = new Dictionary<string, List<Guid>>();

            foreach (var document in documents)
            {
                JToken obj = document.Value;
                if (keys.Length > 0)
                    foreach (var key in keys)
                    {
                        obj = obj?[key];
                        if(JsonCommand.CheckIsString(obj))
                            break;
                    }

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
                        _indexCollection.Add(str, new List<Guid> {document.Id});
                    }
                }
            }
            
            return _indexCollection;
        }
    }
}