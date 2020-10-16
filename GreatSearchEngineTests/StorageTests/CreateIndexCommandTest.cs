using System.Threading.Tasks;
using Core.Storage;
using NUnit.Framework;


namespace GreatSearchEngineTests.StorageTests
{
    public class CreateIndexCommandTest
    {
        private CreateIndexCommand _indexCommand;

        [SetUp]
        public void Setup()
        {
            _indexCommand = new CreateIndexCommand();
        }

        [Test]
        public async Task CreateIndexTest()
        {
            try
            {
                _indexCommand.CreateIndex("TestDb", "Users");
            }
            catch
            {
                Assert.Fail();
            }
        }
    }
}