using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Core.Storage.BinaryStorage;
using GreatSearchEngineTests.Datas;
using NUnit.Framework;

namespace GreatSearchEngineTests.BinaryStorageTests
{
    public class DatabaseTests
    {
        private DocumentDatabase Database { get; set; }
        private List<DocumentModel> DocumentModel { get; set; }

        [SetUp]
        public async Task Setup()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}/binary/";
            await FileOperations.DeleteDirectory(path);
            await FileOperations.CheckOrCreateDirectory(path);
            Database = new DocumentDatabase(new IndexModel("bin_stor", "bin_stor"));
            DocumentModel = Data.SetData(new IndexModel("bin_stor", "bin_stor"));
        }
        
        [Test]
        public void CreateTest()
        {
            foreach (var d in DocumentModel)
            {
                Database.Insert(d);
            }
            
            for (var i = 0; i < 7; i++)
            {
                var actual = Database.Find(DocumentModel[i].Id);
                Assert.AreEqual(DocumentModel[i].Value, actual.Value);
            }
        }
        
        [Test]
        public void UpdateTest()
        {
            foreach (var d in DocumentModel)
            {
                Database.Insert(d);
            }
            
            var value = DocumentModel[1].Value;
            var model = DocumentModel.First();
            model.Value = value;
            Database.Update(model);

            var actual = Database.Find(model.Id);
            Assert.AreEqual(value, actual.Value);
        }
        
        [Test]
        public void DeleteTest()
        {
            foreach (var d in DocumentModel)
            {
                Database.Insert(d);
            }
            
            var model = DocumentModel.First();
            Database.Delete(model);

            var actual = Database.Find(model.Id);
            Assert.IsNull(actual);
        }
    }
}