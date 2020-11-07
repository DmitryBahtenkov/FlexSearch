using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Analyzer;
using Core.Enums;
using Core.Models;
using Core.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        /// <summary>
        /// Поиск по точному совпадению
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="idxName"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<DocumentModel>> SearchMatch(string dbName, string idxName, BaseSearchModel searchModel)
        {
            var docs = await _getDocumentsCommand.Get(dbName, idxName);
            
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
        /// <param name="dbName"></param>
        /// <param name="idxName"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<DocumentModel>> SearchIntersect(string dbName, string idxName, BaseSearchModel searchModel)
        {
            var all = new List<List<int>>();
            var ids = new List<int>();
            var tokens = await _analyzer.Anal(searchModel.Text);
            var docs = await _getDocumentsCommand.Get(dbName, idxName);
            var data = await _getIndexingDocsCommand.Get(dbName, idxName, searchModel.Key);

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