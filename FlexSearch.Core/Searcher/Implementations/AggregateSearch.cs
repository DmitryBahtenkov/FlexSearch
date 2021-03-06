using Core.Analyzer;
using Core.Models;
using Core.Searcher.Interfaces;
using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Storage.Database;

namespace Core.Searcher.Implementations
{
    public class AggregateSearch : ISearch
    {

        private readonly IndexingOperations _indexingOperations;
        private readonly Analyzer.Analyzer _analyzer;

        public AggregateSearch()
        {
            _indexingOperations = new IndexingOperations();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer());
        }

        public SearchType Type => SearchType.Or;

        public async Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, SearchModel searchModel)
        {
            var ids = new List<Guid>();
            var tokens = await _analyzer.Anal(searchModel.Term);

            var idxs = await DatabaseService.GetIndexes(indexModel, searchModel.Key);
            //var docs = await _getOperations.GetDocuments(indexModel);
            //var data = await _indexingOperations.GetIndexesAllKeys(indexModel, searchModel.Key);

            foreach (var dict in idxs)
            {
                foreach (var (k, val) in dict)
                {
                    var items = tokens.Where(token => token == k).Select(_ => val);
                    ids.AddRange(items);
                }

            }

            var result = new List<DocumentModel>();
            foreach (var id in ids.Distinct())
            {
                result.Add(await DatabaseService.FindById(indexModel, id.ToString()));
            }
            return result.Distinct().ToList();
        }

    }
}