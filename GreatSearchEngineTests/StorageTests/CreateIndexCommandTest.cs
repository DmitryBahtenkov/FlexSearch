using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using NUnit.Framework;


namespace GreatSearchEngineTests.StorageTests
{
    public class CreateIndexCommandTest
    {
        private CreateOperations _createOperations;
        private IndexModel IndexModel { get; set; }

        [SetUp]
        public void Setup()
        {
            _createOperations = new CreateOperations();
            IndexModel = new IndexModel("testdb", "users");
        }

        [Test]
        public async Task CreateIndexTest()
        {
            try
            {
                await _createOperations.CreateIndex(IndexModel);
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}