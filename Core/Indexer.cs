using System.Collections.Generic;
using Core.Models;

namespace Core
{
    public class Indexer
    {
        private Dictionary<string, List<long>> _indexCollection;
        private readonly Analyzer _analyzer;

        public Indexer(Analyzer analyzer)
        {
            _analyzer = analyzer;
            _indexCollection = new Dictionary<string, List<long>>();
        }


        public Dictionary<string, List<long>> Add(IList<DocumentModel> documents, string key)
        {
            foreach (var document in documents)
            {
                foreach (var text in _analyzer.Anal(document.Value[key]?.ToObject<string>()))
                {
                    if (_indexCollection.ContainsKey(text))
                    {
                        if(_indexCollection[text].Contains(document.Id))
                            continue;
                        _indexCollection[text].Add(document.Id);
                    }
                    else
                    {
                        _indexCollection.Add(text, new List<long> {document.Id});
                    }
                }
            }
            
            return _indexCollection;
        }
    }
}