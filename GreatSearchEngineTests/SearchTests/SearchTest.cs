using System.Threading.Tasks;
using Core.Models;
using Core.Searcher;
using NUnit.Framework;

namespace GreatSearchEngineTests.SearchTests
{
    public class SearchTest
    {
        private Searcher _searcher;
        [SetUp]
        public void Setup()
        {
            _searcher = new Searcher();
        }

        [Test]
        public async Task SearchTextTest()
        {
            var result = await _searcher.Search("test", "test", new BaseSearchModel
            {
                Key = "Text",
                Text = "Want Get"
            });
            Assert.AreEqual(1, result.Count);
        }
    }
}