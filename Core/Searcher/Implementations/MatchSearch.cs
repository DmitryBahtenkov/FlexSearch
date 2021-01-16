using Core.Analyzer;
using Core.Analyzer.Commands;
using Core.Models;
using Core.Searcher.Interfaces;
using Core.Storage;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Searcher.Implementations
{
    public class MatchSearch : ISearch
    {

        private readonly IndexingOperations _indexingOperations;
        private readonly GetOperations _getOperations;
        private readonly Analyzer.Analyzer _analyzer;

        public MatchSearch()
        {
            _indexingOperations = new IndexingOperations();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer());
            _getOperations = new GetOperations();
        }

        public async Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var docs = await _getOperations.GetDocuments(indexModel);
            var list = new List<DocumentModel>();

            foreach (var doc in docs)
            {
                var val = JsonCommand.GetValueForKey(doc.Value, searchModel.Key);
                if (val is null)
                    continue;
                if (val.ToString().Contains(searchModel.Term.Trim()))
                    list.Add(doc);
            }

            return list;
        }
    }
}
