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
        private readonly GetOperations _getOperations;
        private readonly Analyzer.Analyzer _analyzer;

        public ErrorsSearch()
        {
            _indexingOperations = new IndexingOperations();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer());
            _getOperations = new GetOperations();
        }

        public SearchType Type => SearchType.Errors;
        public async Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var all = new List<List<Guid>>();
            var ids = new List<Guid>();
            var tokens = await _analyzer.Anal(searchModel.Term);
            /*var docs = await _getOperations.GetDocuments(indexModel);
            var data = await _indexingOperations.GetIndexesAllKeys(indexModel, searchModel.Key);*/

            var idxs = await DatabaseService.GetIndexes(indexModel, searchModel.Key);
            foreach (var dict in idxs)
            {
                var intersect = dict.Keys.Where(x=>tokens.Count(y => DamerauLevenshteinDistance.GetDistance(x, y) < 3) != 0);
                foreach (var key in intersect)
                {
                    ids.Add(dict[key]);
                }
            }

            /*if (all.Count == 1)
            {
                ids = all[0];
            }
            else if (all.Count == 2)
            {
                ids.AddRange(all[0].Intersect(all[1]));
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
            }*/

            var result = new List<DocumentModel>();

            foreach (var id in ids.Distinct())
            {
                result.Add(await DatabaseService.FindById(indexModel, id.ToString()));
            }
            return result;
        }
    }
}
