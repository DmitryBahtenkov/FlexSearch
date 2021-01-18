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
            if (_searcher.Type == type)
                return;
            switch (type)
            {
                case SearchType.Fulltext:
                    _searcher = new FullTextSearch();
                    break;
                case SearchType.Errors:
                    _searcher = new ErrorsSearch();
                    break;
                case SearchType.Match:
                    _searcher = new MatchSearch();
                    break;
                case SearchType.Regex:
                    _searcher = new RegexSearch();
                    break;
                case SearchType.Full:
                    _searcher = new AllDocSearch();
                    break;
                case SearchType.Or:
                    _searcher = new AggregateSearch();
                    break;
                case SearchType.Not:
                    _searcher = new NotAndSearch();
                    break;
                default:
                    throw new ArgumentException(type.ToString());
            }
        }
    }
}