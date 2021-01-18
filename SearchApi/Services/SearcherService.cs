using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Searcher;
using Core.Searcher.Implementations;
using Core.Searcher.Interfaces;

namespace SearchApi.Services
{
    public class SearcherService
    {
        private ISearch _searcher;

        public async Task<List<DocumentModel>> Search(IndexModel indexModel, BaseSearchModel searchModel)
        {
            SetSearcher(searchModel.Type);
            return await _searcher.ExecuteSearch(indexModel, searchModel);
        } 

        private void SetSearcher(SearchType type)
        {
            if (_searcher is not null && _searcher?.Type == type)
                return;
            _searcher = type switch
            {
                SearchType.Fulltext => new FullTextSearch(),
                SearchType.Errors => new ErrorsSearch(),
                SearchType.Match => new MatchSearch(),
                SearchType.Regex => new RegexSearch(),
                SearchType.Full => new AllDocSearch(),
                SearchType.Or => new AggregateSearch(),
                SearchType.Not => new NotAndSearch(),
                _ => throw new ArgumentException(type.ToString())
            };
        }
    }
}