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
        private List<DocumentModel> DocumentModel => Data.SetData(new IndexModel("bin_stor", "bin_stor"));

        [SetUp]
        public async Task Setup()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}/binary/";
            await FileOperations.DeleteDirectory(path);
            await FileOperations.CheckOrCreateDirectory(path);
            Database = new DocumentDatabase(path);
        }

        [Test]
        public void CreateTest()
        {
            foreach (var d in DocumentModel)
            {
                Database.Insert(d);
            }
            foreach (var d in DocumentModel)
            {
                var actual = Database.Find(d.Id);
                Assert.AreEqual(d.Value, actual.Value);
            }

        }
        
    }
}