using System;
using Core.Models.Search;
using Core.Searcher.Implementations;
using Core.Searcher.Interfaces;

namespace Core.Searcher
{
    public class SearcherFactory
    {
        private ISearch _searcher;
        
        public ISearch GetSearcher(SearchType type)
        {
            if (_searcher is null || _searcher?.Type != type)
            {
                _searcher = type switch
                {
                    SearchType.Fulltext => new FullTextSearch(),
                    SearchType.Errors => new ErrorsSearch(),
                    SearchType.Match => new MatchSearch(),
                    SearchType.Full => new AllDocSearch(),
                    SearchType.Or => new AggregateSearch(),
                    SearchType.Not => new NotAndSearch(),
                    _ => throw new ArgumentException(type.ToString())
                };
            }

            return _searcher;
        }
    }
}