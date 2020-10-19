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
        private readonly Dictionary<string, List<long>> _indexCollection;
        private readonly Analyzer _analyzer;

        public Indexer(Analyzer analyzer)
        {
            _analyzer = analyzer;
            _indexCollection = new Dictionary<string, List<long>>();
        }


        public async Task<Dictionary<string, List<long>>> Add(IList<DocumentModel> documents)
        {
            foreach (var document in documents)
            {
                if(document == null) continue;
                await IndexJson(document.Value, document.Id);
            }
            
            return _indexCollection;
        }

        private async Task IndexJson(JObject obj, long id)
        {
            foreach (var (key, value) in obj)
            {
                switch (value.Type)
                {
                    case JTokenType.String:
                        await AddIndex(value.ToString(), id);
                        break;
                    case JTokenType.Object:
                        await IndexJson(value.ToObject<JObject>(), id);
                        break;
                }
            }
        }

        private async Task AddIndex(string text, long id)
        {
            foreach (var str in await _analyzer.Anal(text))
            {
                if (_indexCollection.ContainsKey(str))
                {
                    if(_indexCollection[str].Contains(id))
                        return;
                    _indexCollection[str].Add(id);
                }
                else
                {
                    _indexCollection.Add(str, new List<long> {id});
                }
            }
        }
    }
}