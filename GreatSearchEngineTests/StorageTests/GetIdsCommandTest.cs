using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using NUnit.Framework;
using Storage.FileSystem;

namespace GreatSearchEngineTests.StorageTests
{
    public class GetIdsCommandTest
    {
        private GetIdsCommand _getIdsCommand;

        [SetUp]
        public void Setup()
        {
            _getIdsCommand = new GetIdsCommand();
        }

        [Test]
        public async Task GetIdsTest()
        {
            var expected = new[] {0}.ToList();
            var actual = await GetIdsCommand.GetIds("TestIds", "Ids");
            Assert.AreEqual(expected, actual);
        }
    }
}