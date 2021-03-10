using Core.Analyzer;
using Core.Models;
using Core.Searcher.Interfaces;
using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Analyzer.Commands;
using Core.Storage.Database;

namespace Core.Searcher.Implementations
{
    public class ErrorsSearch : ISearch
    {

        private readonly IndexingOperations _indexingOperations;
        private readonly Analyzer.Analyzer _analyzer;

        public ErrorsSearch()
        {
            _indexingOperations = new IndexingOperations();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer());
        }

        public SearchType Type => SearchType.Errors;
        public async Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, SearchModel searchModel)
        {
            var ids = new List<Guid>();
            var tokens = await _analyzer.Anal(searchModel.Term);
            var keys = new List<string>();
            var idxs = await DatabaseService.GetIndexes(indexModel, searchModel.Key);
            foreach (var dict in idxs)
            {
                var keys1 = dict.Keys.Where(x=> tokens
                    .Any(y => DamerauLevenshteinDistance
                        .GetDistance(x, y) < 2))
                    .ToList();

                foreach (var k in keys1)
                {
                    if (!keys.Contains(k))
                    {
                        keys.Add(k);
                    }
                }
            }

            foreach (var dict in idxs)
            {
                if (dict.Keys.Count(x => keys.Contains(x)) >= keys.Count - 2)
                {
                    foreach (var key in keys)
                    {
                        if (dict.ContainsKey(key))
                        {
                            ids.Add(dict[key]);
                        }
                    }
                }
            }
            
            var result = new List<DocumentModel>();

            foreach (var id in ids.Distinct())
            {
                result.Add(await DatabaseService.FindById(indexModel, id.ToString()));
            }
            return result;
        }
    }
}
