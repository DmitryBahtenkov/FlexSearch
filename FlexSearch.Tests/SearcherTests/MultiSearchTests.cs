using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using GreatSearchEngineTests.Datas;
using NUnit.Framework;
using SearchApi.Services;

namespace GreatSearchEngineTests.SearcherTests
{
    public class MultiSearchTests
    {
        private SearcherService SearcherService { get; set; }
        private List<SearchModel> SearchModels { get; set; }
        
        private static IndexModel IndexModel => new IndexModel("test_search", "test_search");
        private readonly string _path = $"{AppDomain.CurrentDomain.BaseDirectory}data/test_search/";

        [SetUp]
        public async Task Setup()
        {
            SearcherService = new SearcherService();
            SearchModels = new List<SearchModel>
            {
                new()
                {
                    Type = SearchType.Fulltext,
                    Key = "Text",
                    Term = "parent mission"
                }
            };
            
            if (!Directory.Exists(_path))
            {
                Data.SetData(IndexModel);
                await Data.SetDataAndDirectoriesForTestGetOperation();
            }
        }
        
        [Test]
        public async Task FullTextAndMatchSearchTest()
        {
            SearchModels = new List<SearchModel>
            {
                new()
                {
                    Type = SearchType.Fulltext,
                    Key = "Text",
                    Term = "parent"
                },
                new ()
                {
                    Type = SearchType.Match,
                    Key = "Name",
                    Term = "Yandex"
                }
            };
            
            var result = await SearcherService.MultiSearch(IndexModel, SearchModels);
            Assert.AreEqual(1, result.Count);
        }
    }
}