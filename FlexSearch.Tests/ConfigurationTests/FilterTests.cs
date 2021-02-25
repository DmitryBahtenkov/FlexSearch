using System.Threading.Tasks;
using Core.Analyzer;
using Core.Helper;
using NUnit.Framework;

namespace GreatSearchEngineTests.ConfigurationTests
{
    public class FilterTests
    {
        [Test]
        public async Task FilterConstructorTest()
        {
            var result = await FilterConstructor.GetFilters();
            Assert.IsNotEmpty(result);
        }
        
        [Test]
        public async Task ExecuteFiltersTest()
        {
            var data = new[] { "Андрей, ", "!Дима", "Richie"};
            var normalizer = new Normalizer();
            var result = await normalizer.Normalize(data);
            Assert.AreEqual(result[0], "андр");
            Assert.AreEqual(result[1], "дим");
            Assert.AreEqual(result[2], "rich");
            
        }
    }
}