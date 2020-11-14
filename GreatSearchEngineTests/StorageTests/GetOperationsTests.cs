using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using NUnit.Framework;

namespace GreatSearchEngineTests.StorageTests
{
    public class GetOperationsTests
    {
        private GetOperations _getOperations;
        private CreateOperations _createOperations;

        [SetUp]
        public async Task Setup()
        {
            _getOperations = new GetOperations();
            _createOperations = new CreateOperations();
            if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}data/test/test"))
                await _createOperations.CreateIndex(new IndexModel("test", "test"));
        }

        [Test]
        public async Task GetDatabasesTest()
        {
            Assert.NotNull(await _getOperations.GetDatabases());
        }
        
        [Test]
        public async Task GetIndexesTest()
        {
            Assert.NotNull(await _getOperations.GetIndexes("test"));
        }
    }
}