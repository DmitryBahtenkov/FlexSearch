using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Helper;
using Core.Models;
using Core.Models.Search;
using Core.Searcher.Implementations;
using Core.Searcher.Interfaces;

namespace SearchApi.Services
{
    public class SearcherService
    {
        private ISearch _searcher;

        public async Task<List<DocumentModel>> MultiSearch(IndexModel indexModel, MultiSearchModel searchModel)
        {
            var results = new List<List<DocumentModel>>();
            foreach (var search in searchModel.Searches)
            {
                SetSearcher(search.Type);
                results.Add(await _searcher.ExecuteSearch(indexModel, search));
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
            SetSearcher(searchModel.Type); 
            
            return GetSorted(searchModel, await _searcher.ExecuteSearch(indexModel, searchModel));
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