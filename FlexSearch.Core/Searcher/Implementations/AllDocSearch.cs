using Core.Analyzer;
using Core.Models;
using Core.Searcher.Interfaces;
using Core.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models.Search;
using Core.Storage.Database;

namespace Core.Searcher.Implementations
{
    public class AllDocSearch : ISearch
    {

        private readonly IndexingOperations _indexingOperations;
        private readonly Analyzer.Analyzer _analyzer;

        public AllDocSearch()
        {
            _indexingOperations = new IndexingOperations();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer());
        }

        public SearchType Type => SearchType.Full;

        public async Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, SearchModel searchModel)
        {
            var list = new List<DocumentModel>();
            foreach (var doc in await DatabaseService.GetAll(indexModel))
            {
                if (JsonConvert.SerializeObject(doc.Value).Contains(searchModel.Term))
                    list.Add(doc);
            }

            return list;
        }

    }
}
