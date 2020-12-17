#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Analyzer;
using Core.Analyzer.Commands;
using Core.Models;
using Core.Storage;
using Newtonsoft.Json;
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
        /// Поиск по операции "И НЕ"
        /// </summary>
        /// <param name="indexModel"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<DocumentModel>> SearchNotAnd(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var docs = await _getOperations.GetDocuments(indexModel);
            var result = new List<DocumentModel>();
            foreach (var doc in docs)
            {
                var value = GetValueForKey(doc.Value, searchModel.Key)?.ToString();
                if(value is null)
                    continue;
                var data = await _analyzer.Anal(value);
                var tokens = await _analyzer.Anal(searchModel.Term);
                var flag = new List<bool>();
                foreach (var dat in data)
                {
                    foreach (var token in tokens)
                    {
                        flag.Add(token == dat);
                    } 
                }
                if(flag.Contains(true))
                    continue;
                result.Add(doc);
            }

            return result.Distinct().ToList();
        }

        /// <summary>
        /// Поиск по операции "ИЛИ"
        /// </summary>
        /// <param name="indexModel"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<DocumentModel>> SearchAggregate(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var all = new List<List<Guid>>();
            var ids = new List<Guid>();
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

            if (all.Count == 1)
            {
                ids = all[0];

            } 
            else if (all.Count == 2)
            {
                foreach (var a in all)
                {
                    ids.AddRange(a);
                }
            }
            else
            { 
                for (var i = 0; i < all.Count - 1; i++)
                {
                    var current = all[i];
                    var union = current.Union(all[i + 1]).ToList();
                    foreach (var item in union.Where(item => !ids.Contains(item)))
                    {
                        ids.Add(item);
                    }
                }
                
            }

            var result = (from doc in docs from id in ids where doc.Id == id select doc).ToList();
            return result.Distinct().ToList();
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
            var list = new List<DocumentModel>();
            
            foreach (var doc in docs)
            {
                var val = GetValueForKey(doc.Value, searchModel.Key);
                if(val is null)
                    continue;
                if(val.ToString().Contains(searchModel.Term))
                    list.Add(doc);
            }

            return list;
        }

        /// <summary>
        /// Полнотекстовый поиск по индексу
        /// </summary>
        /// <param name="indexModel"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<DocumentModel>> SearchIntersect(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var all = new List<List<Guid>>();
            var ids = new List<Guid>();
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

            if (all.Count == 1)
            {
                ids = all[0];

            }
            else if(all.Count == 2)
            {
                foreach (var a in all)
                {
                    ids.AddRange(a);
                }
            }
            else
            {
                for (var i = 0; i < all.Count - 1; i++)
                {
                    var current = all[i];
                    var intersect = current.Intersect(all[i + 1]).ToList();
                    foreach (var item in intersect.Where(item => !ids.Contains(item)))
                    {
                        ids.Add(item);
                    }
                }
            }

            var result = (from doc in docs from id in ids where doc.Id == id select doc).ToList();
            return result;
        }

        /// <summary>
        /// Полнотекстовый поиск с ошибками
        /// </summary>
        /// <param name="indexModel"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<DocumentModel>> SearchWithErrors(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var all = new List<List<Guid>>();
            var ids = new List<Guid>();
            var tokens = await _analyzer.Anal(searchModel.Term);
            var docs = await _getOperations.GetDocuments(indexModel);
            var data = await _indexingOperations.GetIndexesAllKeys(indexModel, searchModel.Key);

            foreach (var dict in data)
            {
                foreach (var (k, val) in dict)
                {
                    var items = tokens.Where(token => DamerauLevenshteinDistance.GetDistance(k, token) <= 3).Select(token => val);
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

        /// <summary>
        /// Поиск по регулярным выражениям
        /// </summary>
        /// <param name="indexModel"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<DocumentModel>> SearchWithRegex(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var list = new List<DocumentModel>();
            foreach (var doc in await _getOperations.GetDocuments(indexModel))
            {
                var val = GetValueForKey(doc.Value, searchModel.Key);
                if(val is null)
                    continue;
                if(Regex.IsMatch(val.ToString(), searchModel.Term))
                    list.Add(doc);
            }

            return list;
        }
        
        /// <summary>
        /// Поиск по всему документу
        /// </summary>
        /// <param name="indexModel"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public async Task<List<DocumentModel>> SearchAllDoc(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var list = new List<DocumentModel>();
            foreach (var doc in await _getOperations.GetDocuments(indexModel))
            {
                if(JsonConvert.SerializeObject(doc.Value).Contains(searchModel.Term))
                    list.Add(doc);
            }

            return list;
        }


        private JToken? GetValueForKey(JToken? token, string key)
        {
            if (JsonCommand.CheckIsString(token))
            {
                if (token.Path.EndsWith(key))
                    return token;
            }
            if(token.Type == JTokenType.Object)
            {
                var o = (JObject) token;
                foreach (var (k, v) in o)
                {
                    if (k == key)
                    {
                        return JsonCommand.CheckIsString(v) ? v : GetValueForKey(v, k);
                    }
                    else
                    {
                        if (v.Type == JTokenType.Array)
                        {
                            return GetValueForKey(v.Last, key);
                        }

                        return GetValueForKey(v, key);
                    }
                }
            }
            else if(token.Type == JTokenType.Array)
            {
                var strs = new List<string>();
                foreach (var t in token)
                {
                    strs.Add(t.ToString());
                }

                var str = string.Join(" ", strs);
                return JToken.Parse(JsonConvert.SerializeObject(str));
            }
            return null;
        }
    }
}