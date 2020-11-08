using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Searcher;
using Core.Storage;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GreatSearchEngineTests.SearchTests
{
    public class SearchTest
    {
        private Searcher _searcher;
        private CreateIndexCommand _createIndexCommand;
        private AddObjectToIndexCommand _addObjectToIndexCommand;
        private IndexingOperations _indexingOperations;
        private IndexModel IndexModel { get; set; }

        [SetUp]
        public void Setup()
        {
            _searcher = new Searcher();
            _createIndexCommand = new CreateIndexCommand();
            _addObjectToIndexCommand = new AddObjectToIndexCommand();
            _indexingOperations = new IndexingOperations();
            
            IndexModel = new IndexModel("test", "test");

            if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}data/{IndexModel}/indexing")) 
                return;
            
            _createIndexCommand.CreateIndex(IndexModel);
            var data = Data.SetData();
            foreach (var item in data)
            {
                _addObjectToIndexCommand.Add(IndexModel, JsonConvert.SerializeObject(item.Value));
            }

            _indexingOperations.SetIndexes(IndexModel);
        }

        [Test]
        public async Task SearchTextTest()
        {
            var result = await _searcher.SearchIntersect(IndexModel, new BaseSearchModel
            {
                Key = "Text",
                Text = "parent"
            });
            Assert.AreEqual(2, result.Count);
        }
        
        [Test]
        public async Task SearchMatchTest()
        {
            var result = await _searcher.SearchMatch(IndexModel, new BaseSearchModel
            {
                Key = "Text",
                Text = "COVID break"
            });
            
            Assert.AreEqual(1, result.Count);
        }
    }
}