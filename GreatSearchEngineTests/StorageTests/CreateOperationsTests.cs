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
    public class CreateOperationsTests
    {
        private CreateOperations CreateOperations { get; set; }
        private IndexModel IndexModel => new IndexModel("test_creation", "test_creation");
        private string Path => $"{AppDomain.CurrentDomain.BaseDirectory}data/";

        [SetUp]
        public void Setup()
        {
            CreateOperations = new CreateOperations();
        }

        [Order(0)]
        [Test]
        public async Task CreateIndexTest()
        {
            try
            {
                await CreateOperations.CreateIndex(IndexModel);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var currentState = Directory.Exists(Path + IndexModel);
            Assert.IsTrue(currentState);
        }
        
        [Order(1)]
        [Test]
        public async Task AddObjectTest()
        {
            var strData = JsonConvert.SerializeObject(new TestModel
            {
                Id = "id", Name = "Dmitry", Text = "Add object test"
            });
            try
            {
                await CreateOperations.AddObject(IndexModel, strData);
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var files = Directory.GetFiles(Path + IndexModel);
            
            Assert.IsNotEmpty(files);
        }
    }
}