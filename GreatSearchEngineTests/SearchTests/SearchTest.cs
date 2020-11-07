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
        private IndexingDocumentsCommand _indexingDocumentsCommand;
        private bool IsSetup { get; set; } = false;
        
        [SetUp]
        public void Setup()
        {
            _searcher = new Searcher();
            _createIndexCommand = new CreateIndexCommand();
            _addObjectToIndexCommand = new AddObjectToIndexCommand();
            _indexingDocumentsCommand = new IndexingDocumentsCommand();

            if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}data/test/test/indexing")) 
                return;
            
            _createIndexCommand.CreateIndex("test", "test");
            var data = Data.SetData();
            foreach (var item in data)
            {
                _addObjectToIndexCommand.Add("test", "test", JsonConvert.SerializeObject(item.Value));
            }

            _indexingDocumentsCommand.Indexing("test", "test");
            IsSetup = true;
        }

        [Test]
        public async Task SearchTextTest()
        {
            var result = await _searcher.SearchIntersect("test", "test", new BaseSearchModel
            {
                Key = "Text",
                Text = "parent"
            });
            Assert.AreEqual(2, result.Count);
        }
        
        [Test]
        public async Task SearchMatchTest()
        {
            var result = await _searcher.SearchMatch("test", "test", new BaseSearchModel
            {
                Key = "Text",
                Text = "COVID break"
            });
            
            Assert.AreEqual(1, result.Count);
        }
    }
}