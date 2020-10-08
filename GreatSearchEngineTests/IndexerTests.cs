using System.Collections.Generic;
using Core;
using Core.Commands;
using Core.Models;
using Core.Normalizers;
using Core.Tokenizers;
using NUnit.Framework;

namespace GreatSearchEngineTests
{
    public class IndexerTests
    {
        private Indexer _indexer;

        [SetUp]
        public void Setup()
        {
            _indexer = new Indexer(new Analyzer(new Tokenizer(), new Normalizer()));
        }

        [Test]
        public void IndexTest()
        {
            var documents = new List<DocumentModel>
            {
                new DocumentModel {Id = 1, Value = "a donut on a glass plate"},
                new DocumentModel {Id = 2, Value = "only the donut donut"},
                new DocumentModel {Id = 3, Value = "listen to the drum machine"}
            };
            
            var expected = new Dictionary<string, List<int>>
            {

            };

            var actual = _indexer.Add(documents);
            Assert.AreEqual(expected, actual);
        }
    }
}