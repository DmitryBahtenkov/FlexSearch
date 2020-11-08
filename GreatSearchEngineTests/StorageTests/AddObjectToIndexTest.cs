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
        private IndexModel IndexModel { get; set; }


        [SetUp]
        public void Setup()
        {
            _addObjectToIndex = new AddObjectToIndexCommand();
            _createIndexCommand = new CreateIndexCommand();
            
            IndexModel = new IndexModel("testdb", "users");

            if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}data/{IndexModel}")) 
                return;
            Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/{IndexModel}");
        }

        [Test]
        public async Task AddTest()
        {
            var item = new TestModel
            {
                Name = "Artem",
                Text = "aAAAAAAAAAAAAA"
            };
            await _addObjectToIndex.Add(IndexModel, JsonConvert.SerializeObject(item));

            using var sr = new StreamReader($"{AppDomain.CurrentDomain.BaseDirectory}data/{IndexModel}/0.json");
            var raw = await sr.ReadToEndAsync();
            var obj = JsonConvert.DeserializeObject<DocumentModel>(raw);
            var result = obj.Value?.ToObject<TestModel>();
            
            Assert.AreEqual(item.Name, result?.Name);
        }
    }
}