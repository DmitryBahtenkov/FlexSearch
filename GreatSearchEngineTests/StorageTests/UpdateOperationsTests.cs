using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using GreatSearchEngineTests.Datas;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GreatSearchEngineTests.StorageTests
{
    public class UpdateOperationsTests
    {
        private UpdateOperations UpdateOperations { get; set; }

        [SetUp]
        public async Task Setup()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/test_updating/test_updating/indexing";
            UpdateOperations = new UpdateOperations();
            if (!Directory.Exists(path))
            {
                Data.SetData(new IndexModel("test_updating", "test_updating"));
                await Data.SetDataAndDirectoriesForTestGetOperation();
            }
        }

        [Order(0)]
        [Test]
        public async Task UpdateObjectTest()
        {
            var newObject = new TestModel
            {
                Id = "4c55e019-a1de-435c-8ebb-e0cf5daa1a70",
                Name = "Name",
                Text = "Aaaaaa"
            };
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/test_updating/test_updating/";

            await UpdateOperations.UpdateObject(new IndexModel("test_updating", "test_updating"), newObject.Id,
                JsonConvert.SerializeObject(newObject));

            using (var sr = new StreamReader(path + $"/{newObject.Id}.json"))
            {
                Assert.IsTrue(sr.ReadToEnd().Contains("Aaaaaa"));
            }
        }

        [Order(1)]
        [Test]
        public async Task RenameIndexTest()
        {
            await UpdateOperations.RenameIndex(new IndexModel("test_updating", "test_updating"), "new_updating");
            Assert.IsTrue(Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}data/test_updating/new_updating/"));
            Directory.Delete($"{AppDomain.CurrentDomain.BaseDirectory}data/test_updating/new_updating/", true);
        }
        
        [Order(2)]
        [Test]
        public async Task RenameDbTest()
        {
            await UpdateOperations.RenameDatabase(new IndexModel("test_updating", "test_updating"), "new_updating");
            Assert.IsTrue(Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}data/new_updating/"));
            Directory.Delete($"{AppDomain.CurrentDomain.BaseDirectory}data/new_updating/", true);
        }
    }
}