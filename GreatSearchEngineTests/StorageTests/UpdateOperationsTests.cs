using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using NUnit.Framework;

namespace GreatSearchEngineTests.StorageTests
{
    public class UpdateOperationsTests
    {
        private UpdateOperations _updateOperations;
        private CreateOperations _createOperations;

        [SetUp]
        public void Setup()
        {
            _updateOperations = new UpdateOperations();
            _createOperations = new CreateOperations();

        }

        [Test]
        public async Task UpdateDbTest()
        {
            try
            {
                if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}data/texting"))
                    await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/texting");
                await _updateOperations.RenameDatabase(Data.IndexModel, "texting");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        
        [Test]
        public async Task UpdateIndexTest()
        {
            if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}data/test/test"))
               await _createOperations.CreateIndex(new IndexModel("test", "test"));
            try
            {
                await _updateOperations.RenameIndex(Data.IndexModel, "newwww");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}