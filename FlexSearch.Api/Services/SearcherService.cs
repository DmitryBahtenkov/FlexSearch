using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Helper;
using Core.Models;
using Core.Searcher.Implementations;
using Core.Searcher.Interfaces;

namespace SearchApi.Services
{
    public class SearcherService
    {
        private ISearch _searcher;

        public async Task<List<DocumentModel>> MultiSearch(IndexModel indexModel, IEnumerable<SearchModel> searchModels)
        {
            var results = new List<List<DocumentModel>>();
            foreach (var searchModel in searchModels)
            {
                SetSearcher(searchModel.Type);
                results.Add(await _searcher.ExecuteSearch(indexModel, searchModel));
            }

            return IntersectHelper.Intersect(new DocumentComparer(), results.ToArray()).ToList();
        }

        public async Task<List<DocumentModel>> Search(IndexModel indexModel, SearchModel searchModel)
        {
            SetSearcher(searchModel.Type);
            var searchResult = await _searcher.ExecuteSearch(indexModel, searchModel);
            if (searchModel.SortDict is null ||  string.IsNullOrEmpty(searchModel.Sort.Key)) 
                return searchResult;
            
            if (searchModel.Sort.Value == 0)
            {
                return searchResult
                    .OrderBy(model => SortingService.GetKeyType(searchModel.Sort.Key, model))
                    .ToList();
            }
            else
            {
                return searchResult
                    .OrderByDescending(model => SortingService.GetKeyType(searchModel.Sort.Key, model))
                    .ToList();
            }

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