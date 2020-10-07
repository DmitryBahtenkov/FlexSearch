using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Tokenizers;
using NUnit.Framework;

namespace GreatSearchEngineTests
{
    public class TokenizerTests
    {
        private ITokenizer _tokenizer; 
        [SetUp]
        public void Setup()
        {
            _tokenizer = new Tokenizer();
        }

        [Test]
        public void DeletePunctuationsTest()
        {
            const string text = "sdfg, sdfgfg! sdfgs? sdf;";
            var expected = "sdfg sdfgfg sdfgs sdf";

            var actual = _tokenizer.DeletePunctuation(text);
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public async Task DeletePunctuationsTestAsync()
        {
            const string text = "sdfg, sdfgfg! sdfgs? sdf;";
            const string expected = "sdfg sdfgfg sdfgs sdf";

            var actual = await _tokenizer.DeletePunctuationAsync(text);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SplitString()
        {
            const string text = "sdfg sdfgfg sdfgs sdf";
            var expected = new[] {"sdfg", "sdfgfg", "sdfgs", "sdf"};
            
            var actual = _tokenizer.SplitString(text).ToArray();
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public async Task SplitStringAsync()
        {
            const string text = "sdfg sdfgfg sdfgs sdf";
            var expected = new[] {"sdfg", "sdfgfg", "sdfgs", "sdf"};
            
            var actual = await _tokenizer.SplitStringAsync(text);
            Assert.AreEqual(expected, actual.ToArray());
        }
    }
}