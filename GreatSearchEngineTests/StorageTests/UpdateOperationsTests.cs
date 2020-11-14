using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using NUnit.Framework;

namespace GreatSearchEngineTests.StorageTests
{
    public class UpdateOperationsTests
    {
        private UpdateOperations _updateOperations;

        [SetUp]
        public void Setup()
        {
            _updateOperations = new UpdateOperations();
        }

        [Test]
        public async Task UpdateDbTest()
        {
            try
            {
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
            try
            {
                await _updateOperations.RenameIndex(Data.IndexModel, "texting");
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}