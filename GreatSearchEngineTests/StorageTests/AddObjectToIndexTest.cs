using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GreatSearchEngineTests.StorageTests
{
    public class AddObjectToIndexTest
    {
        private AddObjectToIndexCommand _addObjectToIndex;
        private CreateIndexCommand _createIndexCommand;


        [SetUp]
        public void Setup()
        {
            _addObjectToIndex = new AddObjectToIndexCommand();
            _createIndexCommand = new CreateIndexCommand();

            if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}data/testdb/users")) 
                return;
            Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/testdb/users");
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

            using var sr = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}data/testdb/users/0.json");
            var raw = await sr.ReadToEndAsync();
            var obj = JsonConvert.DeserializeObject<DocumentModel>(raw);
            var result = obj.Value?.ToObject<TestModel>();
            
            Assert.AreEqual(item.Name, result?.Name);
        }
    }
}