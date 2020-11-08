using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using NUnit.Framework;


namespace GreatSearchEngineTests.StorageTests
{
    public class CreateIndexCommandTest
    {
        private CreateIndexCommand _indexCommand;
        private IndexModel IndexModel { get; set; }

        [SetUp]
        public void Setup()
        {
            _indexCommand = new CreateIndexCommand();
            IndexModel = new IndexModel("testdb", "users");
        }

        [Test]
        public async Task CreateIndexTest()
        {
            try
            {
                await _indexCommand.CreateIndex(IndexModel);
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}