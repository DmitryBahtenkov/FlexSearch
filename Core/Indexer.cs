using System.Collections.Generic;
using Core.Models;
using Core.Normalizers;

namespace Core
{
    public class Indexer
    {
        public Dictionary<string, int[]> IndexCollection { get; set; }
        private readonly Analyzer _analyzer;

        public Indexer(Analyzer analyzer)
        {
            _analyzer = analyzer;
        }


        public void Add(IEnumerable<DocumentModel> documents, string key)
        {
            foreach (var document in documents)
            {
                foreach (var text in _analyzer.Anal(document.Value["key"].ToString()))
                {
                    //я често хуй знает как это реализовать
                }
            }
        }
    }
}