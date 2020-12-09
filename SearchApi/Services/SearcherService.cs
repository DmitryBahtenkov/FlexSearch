using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Searcher;

namespace SearchApi.Services
{
    public class SearcherService
    {
        private readonly Searcher _searcher;

        public SearcherService()
        {
            _searcher = new Searcher();
        }

        public async Task<List<DocumentModel>> Search(IndexModel indexModel, BaseSearchModel searchModel)
        {
            return searchModel.Type switch
            {
                SearchType.Fulltext => await _searcher.SearchIntersect(indexModel, searchModel),
                SearchType.Errors => await _searcher.SearchWithErrors(indexModel, searchModel),
                SearchType.Match => await _searcher.SearchMatch(indexModel, searchModel),
                SearchType.Regex => await _searcher.SearchWithRegex(indexModel, searchModel),
                SearchType.Full => await _searcher.SearchAllDoc(indexModel, searchModel),
                SearchType.Or => await _searcher.SearchAggregate(indexModel, searchModel),
                _ => throw new ArgumentOutOfRangeException(nameof(searchModel.Type), "Неверный тип")
            };
        } 
    }
}