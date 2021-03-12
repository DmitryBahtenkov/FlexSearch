using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Searcher;
using Core.Searcher.Implementations;
using Core.Searcher.Interfaces;
using GreatSearchEngineTests.Datas;
using NUnit.Framework;

namespace GreatSearchEngineTests.SearcherTests
{
    public class SearchTests
    {
        private ISearch Searcher { get; set; }
        private IndexModel IndexModel => new IndexModel("test_search", "test_search");
        private string Path = $"{AppDomain.CurrentDomain.BaseDirectory}data/test_search/";
        [SetUp]
        public async Task Setup()
        {
            if (!Directory.Exists(Path))
            {
                Data.SetData(IndexModel);
                await Data.SetDataAndDirectoriesForTestGetOperation();
            }
        }

        [Test]
        public async Task FullTextSearchTest()
        {
            Searcher = new FullTextSearch();
            var search = new SearchModel
            {
                Type = SearchType.Fulltext,
                Key = "Text",
                Term = "Parent"
            };
            
            var result = await Searcher.ExecuteSearch(IndexModel, search);
            Assert.AreEqual(2, result.Count);
        }
        
        [Test]
        public async Task MatchSearchTest()
        {
            Searcher = new MatchSearch();
            var search = new SearchModel
            {
                Type = SearchType.Match,
                Key = "Text",
                Term = "parent mission"
            };
            
            var result = await Searcher.ExecuteSearch(IndexModel, search);
            Assert.AreEqual(1, result.Count);
        }
        
        [Test]
        public async Task FullDocSearchTest()
        {
            Searcher = new AllDocSearch();
            var search = new SearchModel
            {
                Type = SearchType.Full,
                Term = "I guess you gonna like"
            };
            
            var result = await Searcher.ExecuteSearch(IndexModel, search);
            Assert.AreEqual(1, result.Count);
        }
        
        [Test]
        public async Task ErrorsSearchTest()
        {
            Searcher = new ErrorsSearch();
            var search = new SearchModel
            {
                Type = SearchType.Errors,
                Key = "Text",
                Term = "parnt"
            };
            
            var result = await Searcher.ExecuteSearch(IndexModel, search);
            Assert.AreEqual(2, result.Count);
        }
    }
}