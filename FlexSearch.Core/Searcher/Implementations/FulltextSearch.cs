using Core.Analyzer;
using Core.Models;
using Core.Searcher.Interfaces;
using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.Search;
using Core.Storage.Database;

namespace Core.Searcher.Implementations
{
    public class FullTextSearch : ISearch
    {

        private readonly IndexingOperations _indexingOperations;
        private readonly Analyzer.Analyzer _analyzer;

        public FullTextSearch()
        {
            _indexingOperations = new IndexingOperations();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer());
        }

        public SearchType Type => SearchType.Fulltext;

        public async Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, SearchModel searchModel)
        {
            //var all = new List<List<Guid>>();
            var ids = new List<Guid>();
            var tokens = await _analyzer.Anal(searchModel.Term);
            //var docs = await _getOperations.GetDocuments(indexModel);
            //var data = await _indexingOperations.GetIndexesAllKeys(indexModel, searchModel.Key);

            var idxs = await DatabaseService.GetIndexes(indexModel, searchModel.Key);

            foreach (var dict in idxs)
            {
                var intersect = dict.Keys.Intersect(tokens);
                if (intersect.Count() >= tokens.Count)
                {
                    foreach (var key in intersect)
                    {
                        ids.Add(dict[key]);
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
