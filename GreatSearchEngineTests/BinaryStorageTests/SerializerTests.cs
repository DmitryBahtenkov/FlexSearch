using System.Linq;
using Core.Models;
using Core.Storage.BinaryStorage;
using GreatSearchEngineTests.Datas;
using NUnit.Framework;

namespace GreatSearchEngineTests.BinaryStorageTests
{
    public class SerializerTests
    {
        private DocumentSerializer Serializer { get; set; }
        private DocumentModel DocumentModel { get; set; }

        [SetUp]
        public void Setup()
        {
            Serializer = new DocumentSerializer();
            DocumentModel = Data.SetData(new IndexModel("bin", "stor")).First();
        }

        [Test]
        public void DataLengthTest()
        {
            var bytes = Serializer.Serialize(DocumentModel);
            Assert.IsNotEmpty(bytes);
        }

        [Test]
        public void DataDeserializeTest()
        {
            var bytes = Serializer.Serialize(DocumentModel);
            var model = Serializer.Deserialize(bytes);
            Assert.AreEqual(DocumentModel.Value, model.Value);
        }
    }
}