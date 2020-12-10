using Core.Searcher;
using NUnit.Framework;

namespace GreatSearchEngineTests.SearcherTests
{
    public class AlgorythmTests
    {
        [Test]
        public void DamerauLevenshteinTest()
        {
            var first = "fкул";
            var second = "кит";
            var result = DamerauLevenshteinDistance.GetDistance(first, second);
            Assert.AreEqual(3, result);
        }
    }
}