using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Searcher;
using GreatSearchEngineTests.Datas;
using NUnit.Framework;

namespace GreatSearchEngineTests.SearcherTests
{
    public class SearchTests
    {
        private Searcher Searcher { get; set; }
        private IndexModel IndexModel => new IndexModel("test_search", "test_search");
        private string Path = $"{AppDomain.CurrentDomain.BaseDirectory}data/test_search/test_search";
        [SetUp]
        public async Task Setup()
        {
            if (!Directory.Exists(Path))
            {
                Data.SetData(IndexModel);
                await Data.SetDataAndDirectoriesForTestGetOperation();
            }
            Searcher = new Searcher();
        }

        [Test]
        public async Task FullTextSearchTest()
        {
            var search = new BaseSearchModel
            {
                Type = SearchType.Fulltext,
                Key = "Text",
                Term = "Parent"
            };
            
            var result = await Searcher.SearchIntersect(IndexModel, search);
            Assert.AreEqual(2, result.Count);
        }
        
        [Test]
        public async Task MatchSearchTest()
        {
            var search = new BaseSearchModel
            {
                Type = SearchType.Match,
                Key = "Text",
                Term = "parent mission"
            };
            
            var result = await Searcher.SearchMatch(IndexModel, search);
            Assert.AreEqual(1, result.Count);
        }
        
        [Test]
        public async Task FullDocSearchTest()
        {
            var search = new BaseSearchModel
            {
                Type = SearchType.Full,
                Term = "I guess you gonna like"
            };
            
            var result = await Searcher.SearchAllDoc(IndexModel, search);
            Assert.AreEqual(1, result.Count);
        }
        
        [Test]
        public async Task ErrorsSearchTest()
        {
            var search = new BaseSearchModel
            {
                Type = SearchType.Errors,
                Key = "Text",
                Term = "parnt missin"
            };
            
            var result = await Searcher.SearchWithErrors(IndexModel, search);
            Assert.AreEqual(2, result.Count);
        }
    }
}