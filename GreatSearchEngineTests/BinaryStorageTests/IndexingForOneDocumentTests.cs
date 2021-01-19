using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using GreatSearchEngineTests.Datas;
using NUnit.Framework;

namespace GreatSearchEngineTests.BinaryStorageTests
{
    public class IndexingForOneDocumentTests
    {
        [Test]
        public async Task IndexingTest()
        {
            var operations = new IndexingOperations();
            var model = Data.SetData(null).First();
            var result = await operations.CreateIndexes(model.Value, model);
            Assert.IsNotEmpty(result);
        }
    }
}