using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using GreatSearchEngineTests.Datas;
using NUnit.Framework;

namespace GreatSearchEngineTests.StorageTests
{
    public class GetOperationsTests
    {
        private GetOperations GetOperations => new GetOperations(); 

        [SetUp]
        public async Task Setup()
        {
            Data.SetData(new IndexModel("test_getoper", "test_getoper"));
            await Data.SetDataAndDirectoriesForTestGetOperation();
        }

        [Test]
        public async Task GetDocumentsTest()
        {
            var docs = await GetOperations.GetDocuments(Data.IndexModel);
            Assert.AreEqual(7, docs.Count);
        }
        
        [Test]
        public async Task GetDbsTest()
        {
            var docs = await GetOperations.GetDatabases();
            Assert.IsNotEmpty(docs);
        }
        
        [Test]
        public async Task GetIndexesTest()
        {
            var docs = await GetOperations.GetIndexes("test_getoper");
            Assert.IsNotEmpty(docs);
        }
    }
}