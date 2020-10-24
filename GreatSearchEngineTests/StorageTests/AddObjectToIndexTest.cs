using System.Threading.Tasks;
using Core.Storage;
using Newtonsoft.Json;
using NUnit.Framework;



namespace GreatSearchEngineTests.StorageTests
{
    public class AddObjectToIndexTest
    {
        private AddObjectToIndexCommand _addObjectToIndex;


        [SetUp]
        public void Setup()
        {
            _addObjectToIndex = new AddObjectToIndexCommand();
        }

        [Test]
        public async Task AddTest()
        {
            var item = new TestModel
            {
                Name = "Artem",
                Text = "aAAAAAAAAAAAAA"
            };
            await _addObjectToIndex.Add("testdb", "Users", JsonConvert.SerializeObject(item));

            var obj = await ReadJsonFileCommand.ReadFile($"data/TestDb/Users/0.json");
            var result = obj["Value"]?.ToObject<TestModel>();
            
            Assert.AreEqual(item.Name, result?.Name);
        }
    }
}