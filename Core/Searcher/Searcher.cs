using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Analyzer;
using Core.Models;
using Core.Storage;
using Newtonsoft.Json.Linq;

namespace Core.Searcher
{
    public class Searcher
    {
        private readonly IndexingOperations _indexingOperations;
        private readonly GetOperations _getOperations;
        private readonly Analyzer.Analyzer _analyzer;
        
        public Searcher()
        {
            _indexingOperations = new IndexingOperations();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer());
            _getOperations = new GetOperations();
        }

        /// <summary>
        /// Поиск по точному совпадению
        /// </summary>
        /// <param name="indexModel"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<DocumentModel>> SearchMatch(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var docs = await _getOperations.GetDocuments(indexModel);

            var result = new List<DocumentModel>();
            foreach (var doc in docs)
            {
                if (doc.Value.ContainsKey(searchModel.Key))
                {
                    var jToken = doc.Value[searchModel.Key];
                    
                    if (jToken.Type != JTokenType.Array || jToken.Type != JTokenType.Object )
                    {
                        string text = jToken.ToString();
                        if (text.Contains(searchModel.Text)) 
                            result.Add(doc);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Полнотекстовый поиск по индексу
        /// </summary>
        /// <param name="indexModel"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<DocumentModel>> SearchIntersect(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var all = new List<List<int>>();
            var ids = new List<int>();
            var tokens = await _analyzer.Anal(searchModel.Text);
            var docs = await _getOperations.GetDocuments(indexModel);
            var data = await _indexingOperations.GetIndexesAllKeys(indexModel, searchModel.Key);

            foreach (var dict in data)
            {
                foreach (var (k, val) in dict)
                {
                    all.AddRange(from token in tokens where token == k select val);
                }

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

        public async Task<List<DocumentModel>> SearchWithErrors(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var all = new List<List<int>>();
            var ids = new List<int>();
            var tokens = await _analyzer.Anal(searchModel.Text);
            var docs = await _getOperations.GetDocuments(indexModel);
            var data = await _indexingOperations.GetIndexesAllKeys(indexModel, searchModel.Key);

            foreach (var dict in data)
            {
                foreach (var (k, val) in dict)
                {
                    all.AddRange(from token in tokens
                        where DamerauLevenshteinDistance.GetDistance(k, token) <= 3
                        select val);
                }
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