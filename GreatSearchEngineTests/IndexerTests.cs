using System.Collections.Generic;
using Core;
using Core.Commands;
using Core.Models;
using Core.Normalizers;
using Core.Tokenizers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace GreatSearchEngineTests
{
    public class IndexerTests
    {
        private Indexer _indexer;
        private List<DocumentModel> _documents;

        [SetUp]
        public void Setup()
        {
            _indexer = new Indexer(new Analyzer(new Tokenizer(), new Normalizer()));
            _documents = new List<DocumentModel>();
            SetData();
        }

        [Test]
        public void IndexTest()
        {
            var expected = new Dictionary<string, List<int>>
            {

            };

            var actual = _indexer.Add(_documents, "Text");
            Assert.AreEqual(expected, actual);
        }

        private void SetData()
        {
            var one = new TestModel
            {
                Name = "Irina",
                Text = "I think people should not kill chickens with their knives on inferno"
            };
            var two = new TestModel
            {
                Name = "Sophia",
                Text = "Misha has expressive deep blue eyes with long eyelashes."
            };
            var three = new TestModel
            {
                Name = "Artem",
                Text = "in a few months my family and I will be living in Spain"
            };
            var four = new TestModel
            {
                Name = "Danya",
                Text = "I guess you gonna like my sentence because it's exactly what you need"
            };

            var str1 = JsonConvert.SerializeObject(one);
            var str2 = JsonConvert.SerializeObject(two);
            var str3 = JsonConvert.SerializeObject(three);
            var str4 = JsonConvert.SerializeObject(four);
            
            _documents.Add(new DocumentModel
            {
                Id = 1,
                Value = JObject.Parse(str1)
            });
            _documents.Add(new DocumentModel
            {
                Id = 2,
                Value = JObject.Parse(str2)
            });            _documents.Add(new DocumentModel
            {
                Id = 3,
                Value = JObject.Parse(str3)
            });            _documents.Add(new DocumentModel
            {
                Id = 4,
                Value = JObject.Parse(str4)
            });
        }
    }
}