using System.Threading.Tasks;
using Core.Analyzer;
using NUnit.Framework;

namespace GreatSearchEngineTests.AnalyzerTests
{
    public class TokenizerTests
    {
        private Tokenizer _tokenizer; 
        [SetUp]
        public void Setup()
        {
            _tokenizer = new Tokenizer();
        }

        [Test]
        public async Task DeletePunctuationsTest()
        {
            const string text = "sdfg, sdfgfg! sdfgs? sdf;";
            var expected = "sdfg sdfgfg sdfgs sdf";

            var actual = await _tokenizer.DeletePunctuation(text);
            Assert.AreEqual(expected, actual);
        }
        

        [Test]
        public async Task SplitString()
        {
            const string text = "sdfg sdfgfg sdfgs sdf";
            var expected = new[] {"sdfg", "sdfgfg", "sdfgs", "sdf"};
            
            var actual = await _tokenizer.SplitString(text);
            Assert.AreEqual(expected, actual.ToArray());
        }
    }
}