using Core.Analyzer;
using Core.Analyzer.Commands;
using Core.Models;
using Core.Searcher.Interfaces;
using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Storage.Database;

namespace Core.Searcher.Implementations
{
    public class RegexSearch : ISearch
    {

        private readonly IndexingOperations _indexingOperations;
        private readonly GetOperations _getOperations;
        private readonly Analyzer.Analyzer _analyzer;

        public RegexSearch()
        {
            _indexingOperations = new IndexingOperations();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer());
            _getOperations = new GetOperations();
        }

        public SearchType Type => SearchType.Regex;

        public async Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var list = new List<DocumentModel>();
            foreach (var doc in await DatabaseService.GetAll(indexModel))
            {
                var val = JsonCommand.GetValueForKey(doc.Value, searchModel.Key);
                if (val is null)
                    continue;
                if (Regex.IsMatch(val.ToString(), searchModel.Term))
                    list.Add(doc);
            }

            return list;
        }

    }
}
