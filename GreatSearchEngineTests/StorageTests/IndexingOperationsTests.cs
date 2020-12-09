using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using GreatSearchEngineTests.Datas;
using NUnit.Framework;

namespace GreatSearchEngineTests.StorageTests
{
    public class IndexingOperationsTests
    {
        private IndexingOperations IndexingOperations { get; set; }
        [SetUp]
        public async Task Setup()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/test_indexing/test_indexing/indexing";

            IndexingOperations = new IndexingOperations();
            if (!Directory.Exists(path))
            {
                Data.SetData(new IndexModel("test_indexing", "test_indexing"));
                await Data.SetDataAndDirectoriesForTestGetOperation();
            }
        }

        [Order(0)]
        [Test]
        public async Task SetIndexesTest()
        {
            await IndexingOperations.SetIndexes(new IndexModel("test_indexing", "test_indexing"));
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/test_indexing/test_indexing/indexing";
            var files = Directory.GetFiles(path);
            Assert.IsNotEmpty(files);
        }
        
        [Order(1)]
        [Test]
        public async Task GetIndexesTest()
        {
            var docs = await IndexingOperations.GetIndexesAllKeys(new IndexModel("test_indexing", "test_indexing"), "Text");
            Assert.IsNotEmpty(docs);
        }
    }
}