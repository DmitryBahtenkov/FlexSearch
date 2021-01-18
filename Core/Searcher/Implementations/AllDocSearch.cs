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

namespace Core.Searcher.Implementations
{
    public class AllDocSearch : ISearch
    {

        private readonly IndexingOperations _indexingOperations;
        private readonly GetOperations _getOperations;
        private readonly Analyzer.Analyzer _analyzer;

        public AllDocSearch()
        {
            _indexingOperations = new IndexingOperations();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer());
            _getOperations = new GetOperations();
        }

        public SearchType Type => SearchType.Full;

        public async Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var list = new List<DocumentModel>();
            foreach (var doc in await _getOperations.GetDocuments(indexModel))
            {
                if (JsonConvert.SerializeObject(doc.Value).Contains(searchModel.Term))
                    list.Add(doc);
            }

            return list;
        }

    }
}
