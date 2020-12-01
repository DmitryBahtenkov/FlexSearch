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
            switch (searchModel.Type)
            {
                case SearchType.Fulltext:
                    return await _searcher.SearchIntersect(indexModel, searchModel);
                case SearchType.Errors:
                    return await _searcher.SearchWithErrors(indexModel, searchModel);
                case SearchType.Match:
                    return await _searcher.SearchMatch(indexModel, searchModel);
                default:
                    throw new ArgumentOutOfRangeException(nameof(searchModel.Type), "Неверный тип");
            }
        } 
    }
}