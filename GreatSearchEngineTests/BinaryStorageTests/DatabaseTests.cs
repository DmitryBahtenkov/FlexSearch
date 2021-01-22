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
        private DocumentDatabase Database1 { get; set; }
        private DocumentDatabase Database2 { get; set; }
        private DocumentDatabase Database3 { get; set; }

        private List<DocumentModel> DocumentModel { get; set; }

        [SetUp]
        public async Task Setup()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/bin_stor";
            try
            {
                await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/bin_stor");
                await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/bin_stor1");
                await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/bin_stor2");

            }
            catch
            {
                
            }
            DocumentModel = Data.SetData(new IndexModel("bin_stor", "bin_stor"));
        }
        
        [Test]
        [Order(0)]
        public void CreateTest()
        {
            Database1 = new DocumentDatabase(new IndexModel("bin_stor", "bin_stor"));
            foreach (var d in DocumentModel)
            {
                Database1.Insert(d);
            }
            
            for (var i = 0; i < 7; i++)
            {
                var actual = Database1.Find(DocumentModel[i].Id);
                Assert.AreEqual(DocumentModel[i].Value, actual.Value);
            }
        }
        
        [Test]
        [Order(1)]
        public void UpdateTest()
        {
            Database2 = new DocumentDatabase(new IndexModel("bin_stor1", "bin_stor1"));
            foreach (var d in DocumentModel)
            {
                Database2.Insert(d);
            }
            
            var value = DocumentModel[1].Value;
            var model = DocumentModel.First();
            model.Value = value;
            Database2.Update(model);

            var actual = Database2.Find(model.Id);
            Assert.AreEqual(value, actual.Value);
        }
        
        [Test]
        [Order(2)]
        public void DeleteTest()
        {
            Database3 = new DocumentDatabase(new IndexModel("bin_stor2", "bin_stor2"));
            foreach (var d in DocumentModel)
            {
                Database3.Insert(d);
            }
            
            var model = DocumentModel.First();
            Database3.Delete(model);

            var actual = Database3.Find(model.Id);
            Assert.IsNull(actual);
        }
    }
}