﻿using Core.Analyzer;
using Core.Models;
using Core.Searcher.Interfaces;
using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Searcher.Implementations
{
    public class AggregateSearch : ISearch
    {

        private readonly IndexingOperations _indexingOperations;
        private readonly GetOperations _getOperations;
        private readonly Analyzer.Analyzer _analyzer;

        public AggregateSearch()
        {
            _indexingOperations = new IndexingOperations();
            _analyzer = new Analyzer.Analyzer(new Tokenizer(), new Normalizer());
            _getOperations = new GetOperations();
        }

        public SearchType Type => SearchType.Or;

        public async Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, BaseSearchModel searchModel)
        {
            var all = new List<List<Guid>>();
            var ids = new List<Guid>();
            var tokens = await _analyzer.Anal(searchModel.Term);
            var docs = await _getOperations.GetDocuments(indexModel);
            var data = await _indexingOperations.GetIndexesAllKeys(indexModel, searchModel.Key);

            foreach (var dict in data)
            {
                foreach (var (k, val) in dict)
                {
                    var items = tokens.Where(token => token == k).Select(_ => val);
                    all.AddRange(items);
                }

            }

            if (all.Count == 1)
            {
                ids = all[0];

            }
            else if (all.Count == 2)
            {
                foreach (var a in all)
                {
                    ids.AddRange(a);
                }
            }
            else
            {
                for (var i = 0; i < all.Count - 1; i++)
                {
                    var current = all[i];
                    var union = current.Union(all[i + 1]).ToList();
                    foreach (var item in union.Where(item => !ids.Contains(item)))
                    {
                        ids.Add(item);
                    }
                }

            }

            var result = (from doc in docs from id in ids where doc.Id == id select doc).ToList();
            return result.Distinct().ToList();
        }

    }
}