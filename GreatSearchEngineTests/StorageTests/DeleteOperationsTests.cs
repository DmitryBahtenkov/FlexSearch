using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Core.Storage.Database;
using NUnit.Framework;
using SearchApi.Services;

namespace GreatSearchEngineTests.StorageTests
{
    public class DeleteOperationsTests
    {
        private IndexModel IndexModel => new IndexModel("test_deletion", "test_deletion");
        private string Path => $"{AppDomain.CurrentDomain.BaseDirectory}data/";
        private DatabaseService Service { get; set; }
        
        [SetUp]
        public void Setup()
        {
            Service = new DatabaseService();
            Directory.CreateDirectory(Path + IndexModel.DatabaseName);
        }

        
        [Order(0)]
        [Test]
        public async Task DeleteIndex()
        {
            try
            {
                await Service.DeleteIndex(IndexModel);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            var state = File.Exists(Path + IndexModel + ".col");
            Assert.IsFalse(state);
            state = File.Exists(Path + IndexModel + ".pidx");
            Assert.IsFalse(state);
            state = File.Exists(Path + IndexModel + ".sidx");
            Assert.IsFalse(state);
        }
        
        [Order(2)]
        [Test]
        public async Task DeleteDatabase()
        {
            try
            {
                await Service.DeleteDatabase(IndexModel.DatabaseName);
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