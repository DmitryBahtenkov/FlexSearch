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
        private readonly Tokenizer _tokenizer;
        
        public Searcher()
        {
            _indexingOperations = new IndexingOperations();
            _tokenizer = new Tokenizer();
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
            
            var result = (
                from doc in docs 
                where doc.Value.ContainsKey(searchModel.Key) 
                let jToken = doc.Value[searchModel.Key] 
                where jToken?.Type == JTokenType.String 
                let text = jToken.ToString() 
                where text.Contains(searchModel.Text) 
                select doc).ToList();

            return result;
        }

        /// <summary>
        /// Полнотекстовый поиск по индексу
        /// </summary>
        /// <param name="indexModel"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<DocumentModel>> SearchIntersect(IndexModel indexModel, BaseSearchModel searchModel, string lang)
        {
            var all = new List<List<int>>();
            var ids = new List<int>();
            var tokens = await _tokenizer.Tokenize(searchModel.Text, lang);
            var docs = await _getOperations.GetDocuments(indexModel);
            var data = await _indexingOperations.GetIndexes(indexModel, searchModel.Key);

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