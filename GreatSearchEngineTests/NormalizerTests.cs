using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Normalizers;
using NUnit.Framework;

namespace GreatSearchEngineTests
{
    public class NormalizerTests
    {
        private INormalizer _normalizer;
        private IList<string> _data;

        [SetUp]
        public void Setup()
        {
            _normalizer = new Normalizer();
        }

        [Test]
        public void DeleteStopWordsTest()
        {
            _data = new List<string> {"a", "fish", "add", "the", "resume"};
            var expected = new List<string> {"fish", "add", "resume"};
            var actual = _normalizer.DeleteStopWords(_data);
            
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public async Task DeleteStopWordsAsyncTest()
        {
            _data = new List<string> {"a", "fish", "add", "the", "resume"};
            var expected = new List<string> {"fish", "add", "resume"};
            var actual = await _normalizer.DeleteStopWordsAsync(_data);
            
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void ToLowerCaseTest()
        {
            _data = new List<string> {"A", "fiSh", "ADD", "ThE", "rEsume"};
            var expected = new List<string> {"a", "fish", "add", "the", "resume"};
            var actual = _normalizer.ToLowerCase(_data);
            
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public async Task ToLowerCaseAsyncTest()
        {
            _data = new List<string> {"A", "fiSh", "ADD", "ThE", "rEsume"};
            var expected = new List<string> {"a", "fish", "add", "the", "resume"};
            var actual = await _normalizer.ToLowerCaseAsync(_data);
            
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public void StemTokensTest()
        {
            _data = new List<string> { "fishes", "added", "resumes", "babies"};
            var expected = new List<string> {"fish", "ad", "resum", "babi"};
            var actual = _normalizer.StemTokens(_data);
            
            Assert.AreEqual(expected, actual);
        }
        
        [Test]
        public async Task StemTokensAsyncTest()
        {
            _data = new List<string> { "fishes", "added", "resumes", "babies"};
            var expected = new List<string> {"fish", "ad", "resum", "babi"};
            var actual = await _normalizer.StemTokensAsync(_data);
            
            Assert.AreEqual(expected, actual);
        }
    }
}