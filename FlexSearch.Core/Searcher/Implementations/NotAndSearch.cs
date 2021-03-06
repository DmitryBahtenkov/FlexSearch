﻿using Core.Analyzer;
using Core.Models;
using Core.Searcher.Interfaces;
using Core.Storage;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Analyzer.Commands;
using Core.Models.Search;
using Core.Storage.Database;

namespace Core.Searcher.Implementations
{
    public class NotAndSearch : ISearch
    {

        private readonly IndexingOperations _indexingOperations;
        private readonly Analyzer.Analyzer _analyzer;

        public NotAndSearch()
        {
            _indexingOperations = new IndexingOperations();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer());
        }

        public SearchType Type => SearchType.Not;

        public async Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, SearchModel searchModel)
        {
            var docs = await DatabaseService.GetAll(indexModel);
            var result = new List<DocumentModel>();
            foreach (var doc in docs)
            {
                var value = JsonCommand.GetValueForKey(doc.Value, searchModel.Key)?.ToString();
                if (value is null)
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
                if (flag.Contains(true))
                    continue;
                result.Add(doc);
            }

            return result.Distinct().ToList();
        }
    }
}
