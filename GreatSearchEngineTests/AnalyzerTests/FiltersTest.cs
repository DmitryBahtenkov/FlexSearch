using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Analyzer.Filters;
using Core.Enums;
using NUnit.Framework;


namespace GreatSearchEngineTests.AnalyzerTests
{
    public class FiltersTest
    {
        private IFilter _filter;
        private IList<string> _data;

        [Test]
        public async Task LowerCaseTest()
        {
            _filter = new LowerCaseFilter();
            _data = new List<string> {"ABsdu", "erpioJIOS", "sdf", ""};
            var expected = new List<string> {"absdu", "erpiojios", "sdf", ""};
            var actual = await _filter.Execute(_data);
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public async Task StemmerTest()
        {
            _filter = new StemmerFilter();
            _data = new List<string> {"babies", "machines", "fishes", "12"};
            var expected = new List<string> {"babi", "machin", "fish", "12"};
            var actual = await _filter.Execute(_data);
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public async Task StopWordTest()
        {
            _filter = new StopWordsFilter(Languages.English);
            _data = new List<string> {"a", "the", "machine", "penis"};
            var expected = new List<string> {"machine", "penis"};
            var actual = await _filter.Execute(_data);
            Assert.AreEqual(expected, actual);
        }
    }
}