using System.Threading.Tasks;
using NUnit.Framework;
using Storage.FileSystem;

namespace GreatSearchEngineTests.StorageTests
{
    public class AddObjectToIndexTest
    {
        private AddObjectToIndex _addObjectToIndex;
        private ReadJsonFileCommand _readJsonFileCommand;

        [SetUp]
        public void Setup()
        {
            _addObjectToIndex = new AddObjectToIndex();
            _readJsonFileCommand = new ReadJsonFileCommand();
        }

        [Test]
        public async Task AddTest()
        {
            await _addObjectToIndex.Add("TestDb", "Users", new TestModel
            {
                Name = "Artem",
                Text = "aAAAAAAAAAAAAA"
            });

            var result = await ReadJsonFileCommand.ReadFile($"data/TestDb/Users/0.json");
        }
    }
}