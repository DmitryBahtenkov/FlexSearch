using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Analyzer;
using Core.Models;
using NUnit.Framework;

namespace GreatSearchEngineTests.AnalyzerTests
{
    public class IndexerTests
    {
        private Indexer _indexer;
        private List<DocumentModel> _documents;

        [SetUp]
        public void Setup()
        {
            _indexer = new Indexer(new Analyzer(new Tokenizer(), new Normalizer()));
            _documents = Data.SetData();
            
        }

        [Test]
        public async Task IndexTest()
        {
            var actual = await _indexer.AddDocuments(_documents, "Text");
            Assert.AreEqual(47, actual.Count);
        }

        
    }
}