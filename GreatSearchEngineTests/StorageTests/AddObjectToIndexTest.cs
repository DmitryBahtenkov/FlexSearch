using System.Threading.Tasks;
using Core.Storage;
using Newtonsoft.Json;
using NUnit.Framework;



namespace GreatSearchEngineTests.StorageTests
{
    public class AddObjectToIndexTest
    {
        private AddObjectToIndexCommand _addObjectToIndex;
        private ReadJsonFileCommand _readJsonFileCommand;

        [SetUp]
        public void Setup()
        {
            _addObjectToIndex = new AddObjectToIndexCommand();
            _readJsonFileCommand = new ReadJsonFileCommand();
        }

        [Test]
        public async Task AddTest()
        {
            await _addObjectToIndex.Add("TestDb", "Users", JsonConvert.SerializeObject(new TestModel
            {
                Name = "Artem",
                Text = "aAAAAAAAAAAAAA"
            }));

            var result = await ReadJsonFileCommand.ReadFile($"data/TestDb/Users/0.json");
        }
    }
}