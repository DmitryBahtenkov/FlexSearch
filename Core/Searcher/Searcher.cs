using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Analyzer;
using Core.Enums;
using Core.Models;
using Core.Storage;

namespace Core.Searcher
{
    public class Searcher
    {
        private readonly GetIndexingDocsCommand _getIndexingDocsCommand;
        private readonly GetDocumentsCommand _getDocumentsCommand;
        private readonly Analyzer.Analyzer _analyzer;
        

        public Searcher()
        {
            _getIndexingDocsCommand = new GetIndexingDocsCommand();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer(Languages.English));
            _getDocumentsCommand = new GetDocumentsCommand();
        }

        private IEnumerable<DocumentModel> Intersect(ICollection<DocumentModel> firstArr,
            ICollection<DocumentModel> secondArr)
        {
            var maxLen = firstArr.Count > secondArr.Count ? firstArr.Count : secondArr.Count;

            var result = firstArr.Intersect(secondArr);

            return result;
        }

        public async Task<List<DocumentModel>> Search(string dbName, string idxName, string key, string text)
        {
            var all = new List<List<int>>();
            var ids = new List<int>();
            var tokens = await _analyzer.Anal(text);
            var docs = await _getDocumentsCommand.Get(dbName, idxName);
            var data = await _getIndexingDocsCommand.Get(dbName, idxName, key);

            foreach (var (k, val) in data)
            {
                all.AddRange(from token in tokens where token == k select val);
            }
            


            for (var i = 0; i < all.Count - 1; i++)
            {
                var current = all[i];
                ids.AddRange(current.Intersect(all[i+1]));
            }
            
            if (all.Count == 1)
            {
                ids = all[0];
            }

            var result = (from doc in docs from id in ids where doc.Id == id select doc).ToList();
            return result;
        }
        
    }
}