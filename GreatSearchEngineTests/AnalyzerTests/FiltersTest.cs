/*using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Analyzer.Filters;
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
            _data = new List<string> {"babies", "machines", "fishes", "12", "членистоногий", "боги"};
            var expected = new List<string> {"bab", "machin", "fish", "12", "членистоног", "бог"};
            var actual = await _filter.Execute(_data);
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public async Task StopWordTest()
        {
            _filter = new StopWordsFilter();
            _data = new List<string> {"a", "the", "machine", "penis", "я", "тысяч", "vois", "une"};
            var expected = new List<string> {"machine", "penis"};
            var actual = await _filter.Execute(_data);
            Assert.AreEqual(expected, actual);
        }
    }
}*/