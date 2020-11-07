using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Storage;
using NUnit.Framework;


namespace GreatSearchEngineTests.StorageTests
{
    public class GetIdsCommandTest
    {
        private GetIdsCommand _getIdsCommand;

        [SetUp]
        public void Setup()
        {
            _getIdsCommand = new GetIdsCommand();
            
            if (Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}data/TestIds/Ids")) 
                return;
            Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/TestIds/Ids");
            File.Create($"{AppDomain.CurrentDomain.BaseDirectory}data/TestIds/Ids/0.json").Close();
        }

        [Test]
        public async Task GetIdsTest()
        {
            var expected = new[] {0}.ToList();
            var actual = await _getIdsCommand.GetIds("TestIds", "Ids");
            Assert.AreEqual(expected, actual);
        }
    }
}