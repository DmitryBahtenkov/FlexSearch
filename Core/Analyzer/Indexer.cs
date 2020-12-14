using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    }

                if(obj is null) continue;
                if (JsonCommand.CheckIsString(obj))
                {
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
                else if(obj.Type == JTokenType.Array)
                {
                    var strs = new List<string>();
                    foreach (var t in obj)
                    {
                        if(t.Type == JTokenType.String)
                            strs.Add(t.ToString());
                    }
                    foreach (var str in await _analyzer.Anal(string.Join(" ", strs)))
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
            }
            
            return _indexCollection;
        }
    }
}