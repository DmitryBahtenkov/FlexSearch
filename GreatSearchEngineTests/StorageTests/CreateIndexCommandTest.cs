using System.Threading.Tasks;
using NUnit.Framework;
using Storage.FileSystem;

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