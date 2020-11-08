using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using NUnit.Framework;


namespace GreatSearchEngineTests.StorageTests
{
    /*public class GetIdsCommandTest
    {
        private GetIdsCommand _getIdsCommand;
        private IndexModel IndexModel { get; set; }

        [SetUp]
        public void Setup()
        {
            _getIdsCommand = new GetIdsCommand();
            IndexModel = new IndexModel("testids", "ids");
            
            if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}data/{IndexModel}")) 
                return;
            Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/{IndexModel}");
            File.Create($"{AppDomain.CurrentDomain.BaseDirectory}data/{IndexModel}/0.json").Close();
        }

        [Test]
        public async Task GetIdsTest()
        {
            var expected = new[] {0}.ToList();
            var actual = await _getIdsCommand.GetIds(IndexModel);
            Assert.AreEqual(expected, actual);
        }
    }*/
}