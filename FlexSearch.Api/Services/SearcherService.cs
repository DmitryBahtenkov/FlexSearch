using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Helper;
using Core.Models;
using Core.Models.Search;
using Core.Searcher;
using Core.Searcher.Implementations;
using Core.Searcher.Interfaces;

namespace SearchApi.Services
{
    public class SearcherService
    {
        private readonly SearcherFactory _searcherFactory = new ();

        public async Task<List<DocumentModel>> MultiSearch(IndexModel indexModel, MultiSearchModel searchModel)
        {
            var results = new List<List<DocumentModel>>();
            foreach (var search in searchModel.Searches)
            {
                results.Add(await _searcherFactory.GetSearcher(search.Type).ExecuteSearch(indexModel, search));
            }

            List<DocumentModel> searchResult;
            switch (searchModel.QueryType)
            {
                case QueryType.Or:
                    searchResult = CollectionsHelper.Union(new DocumentComparer(), results.ToArray()).ToList();
                    break;
                case QueryType.And:
                    searchResult = CollectionsHelper.Intersect(new DocumentComparer(), results.ToArray()).ToList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return GetSorted(searchModel, searchResult);
        }

        public async Task<List<DocumentModel>> Search(IndexModel indexModel, SearchModel searchModel)
        {
            return GetSorted(searchModel, await _searcherFactory.GetSearcher(searchModel.Type).ExecuteSearch(indexModel, searchModel));
        } 

        

        private static List<DocumentModel> GetSorted(BaseSortingModel sortingModel, List<DocumentModel> documents)
        {
            if (sortingModel.SortDict is null ||  string.IsNullOrEmpty(sortingModel.Sort.Key)) 
                return documents;
            
            if (sortingModel.Sort.Value == 0)
            {
                return documents
                    .OrderBy(model => SortingService.GetKeyType(sortingModel.Sort.Key, model))
                    .ToList();
            }

            return documents
                .OrderByDescending(model => SortingService.GetKeyType(sortingModel.Sort.Key, model))
                .ToList();
        }
    }
}