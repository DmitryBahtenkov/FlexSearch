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

            return (from doc in docs 
                    where doc.Value.ContainsKey(searchModel.Key) 
                    let jToken = doc.Value[searchModel.Key] 
                    where jToken?.Type == JTokenType.String 
                    let text = jToken.ToString() 
                    where text.Contains(searchModel.Term) 
                    select doc).ToList();
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
            var tokens = await _analyzer.Anal(searchModel.Term);
            var docs = await _getOperations.GetDocuments(indexModel);
            var data = await _indexingOperations.GetIndexesAllKeys(indexModel, searchModel.Key);

            foreach (var dict in data)
            {
                foreach (var (k, val) in dict)
                {
                    var items = tokens.Where(token => token == k).Select(token => val);
                    all.AddRange(items);
                }

            }

            for (var i = 0; i < all.Count - 1; i++)
            {
                var current = all[i];
                var intersect = current.Intersect(all[i + 1]).ToList();
                foreach (var item in intersect.Where(item => !ids.Contains(item)))
                {
                    ids.Add(item);
                }
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
            var tokens = await _analyzer.Anal(searchModel.Term);
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
                var intersect = current.Intersect(all[i + 1]).ToList();
                foreach (var item in intersect.Where(item => !ids.Contains(item)))
                {
                    ids.Add(item);
                }
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