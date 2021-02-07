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
        public async Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var ids = new List<Guid>();
            var tokens = await _analyzer.Anal(searchModel.Term);

            var idxs = await DatabaseService.GetIndexes(indexModel, searchModel.Key);
            foreach (var dict in idxs)
            {
                var keys = dict.Keys.Where(x=> tokens.Any(y => DamerauLevenshteinDistance.GetDistance(x, y) < 3));
                var intersect = keys.Intersect(tokens).ToArray();
                if (intersect.Length < tokens.Count) continue;
                ids.AddRange(intersect.Select(key => dict[key]));
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
