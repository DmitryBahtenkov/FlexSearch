using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using NUnit.Framework;

namespace GreatSearchEngineTests.StorageTests
{
    public class DeleteOperationsTests
    {
        private IndexModel IndexModel => new IndexModel("test_deletion", "test_deletion");
        private string Path => $"{AppDomain.CurrentDomain.BaseDirectory}data/";
        
        [SetUp]
        public void Setup()
        {
            Directory.CreateDirectory(Path + IndexModel);
            File.Create(Path + IndexModel + "/id.json").Close();   
        }

        [Order(0)]
        [Test]
        public async Task DeleteObjectTest()
        {
            try
            {
                await DeleteOperations.DeleteObjectById(IndexModel, "id");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var state = File.Exists(Path + IndexModel + "/id.json");
            Assert.IsFalse(state);
        }
        
        [Order(1)]
        [Test]
        public async Task DeleteIndex()
        {
            try
            {
                await DeleteOperations.DeleteIndex(IndexModel);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var state = Directory.Exists(Path + IndexModel);
            Assert.IsFalse(state);
        }
        
        [Order(2)]
        [Test]
        public async Task DeleteDatabase()
        {
            try
            {
                await DeleteOperations.DeleteDatabase(IndexModel.DatabaseName);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var state = Directory.Exists(Path + IndexModel.DatabaseName);
            Assert.IsFalse(state);
        }
    }
}