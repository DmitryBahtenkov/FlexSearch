using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Analyzer.Commands;
using Core.Enums;
using NUnit.Framework;

namespace GreatSearchEngineTests.AnalyzerTests
{
    public class CommandTests
    {
        [Test]
        public async Task GetStopWordsTest()
        {
            var expected = new List<string> {"i", "me", "my"};
            var actual = await GetStopWordsCommand.GetStopWords(Languages.English);
            Assert.AreEqual(expected[0], actual[0]);
        }
    }
}