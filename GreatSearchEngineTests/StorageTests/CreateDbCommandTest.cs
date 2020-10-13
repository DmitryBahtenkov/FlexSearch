using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Storage.FileSystem;

namespace GreatSearchEngineTests.StorageTests
{
    public class CreateDbCommandTest
    {
        private CreateDbCommand _command;

        [SetUp]
        public void Setup()
        {
            _command = new CreateDbCommand();
        }

        [Test]
        public void CreateDbTest()
        {
            try
            {
                _command.CreateDb("TestDb");
            }
            catch
            {
                Assert.Fail();
            }
            
        }
    }
}