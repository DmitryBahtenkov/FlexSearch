using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Core.Storage.BinaryStorage;
using Core.Storage.Database;
using GreatSearchEngineTests.Datas;
using NUnit.Framework;

namespace GreatSearchEngineTests.BinaryStorageTests
{
    public class DatabaseTests
    {
        private DocumentDatabase Database1 { get; set; }
        private DocumentDatabase Database2 { get; set; }
        private DocumentDatabase Database3 { get; set; }
        private DocumentDatabase Database4 { get; set; }


        private List<DocumentModel> DocumentModel { get; set; }

        [SetUp]
        public async Task Setup()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/bin_stor";
            try
            {
                await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/bin_stor");
            }
            catch
            {
                // ignored
            }
            
            try
            {
                await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/bin_stor1");
            }
            catch
            {
                // ignored
            }
            
            try
            {
                await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/bin_stor2");
            }
            catch
            {
                // ignored
            }
            
            try
            {
                await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/bin_stor3");
            }
            catch
            {
                // ignored
            }

            DocumentModel = Data.SetData(new IndexModel("bin_stor", "bin_stor"));
        }
        
        [Test]
        [Order(0)]
        public async Task CreateTest()
        {
            Database1 = new DocumentDatabase(new IndexModel("bin_stor", "bin_stor"));
            foreach (var d in DocumentModel)
            {
                await Database1.Insert(d);
            }
            
            for (var i = 0; i < 7; i++)
            {
                var actual = await Database1.Find(DocumentModel[i].Id);
                Assert.AreEqual(DocumentModel[i].Value, actual.Value);
            }
        }
        
        [Test]
        [Order(1)]
        public async Task UpdateTest()
        {
            Database2 = new DocumentDatabase(new IndexModel("bin_stor1", "bin_stor1"));
            foreach (var d in DocumentModel)
            {
                await Database2.Insert(d);
            }
            
            var value = DocumentModel[1].Value;
            var model = DocumentModel.First();
            model.Value = value;
            await Database2.Update(model);

            var actual = await Database2.Find(model.Id);
            Assert.AreEqual(value, actual.Value);
        }
        
        [Test]
        [Order(2)]
        public async Task DeleteTest()
        {
            Database3 = new DocumentDatabase(new IndexModel("bin_stor2", "bin_stor2"));
            foreach (var d in DocumentModel)
            {
                await Database3.Insert(d);
            }
            
            var model = DocumentModel.First();
            await Database3.Delete(model);

            var actual = await Database3.Find(model.Id);
            Assert.IsNull(actual);
        }
        
        [Test]
        [Order(2)]
        public async Task GetAllTest()
        {
            Database4 = new DocumentDatabase(new IndexModel("bin_stor3", "bin_stor3"));
            foreach (var d in DocumentModel)
            {
                await Database4.Insert(d);
            }

            var idx = await Database4.GetIndexes("Text");

            var actual = await Database4.GetAllDocuments();
            foreach (var model in DocumentModel)
            {
                Assert.IsTrue(actual.Select(x=>x.Id).Contains(model.Id));
            }
        }
    }
}