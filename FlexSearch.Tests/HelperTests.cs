using System.Collections.Generic;
using System.Linq;
using Core.Helper;
using Core.Models;
using NUnit.Framework;

namespace GreatSearchEngineTests
{
    public class HelperTests
    {
        [Test]
        public void IntersectTest()
        {
            var arr1 = new[] {"1", "23", "11", "32"};
            var arr2 = new[] {"2", "23", "41", "32"};
            var arr3 = new[] {"13", "23", "113", "31"};
            var arr4 = new[] {"112", "23", "111", "312"};

            var actual = CollectionsHelper.Intersect(null,arr1, arr2, arr3, arr4).ToArray();
            Assert.IsNotEmpty(actual);
            Assert.AreEqual("23", actual.First());
        }
        
        [Test]
        public void IntersectDocumentTest()
        {
            var arr = Datas.Data.SetData(new IndexModel("", ""));
            var arr1 = new List<DocumentModel> {arr[5], arr[6]};
            var arr2 = new List<DocumentModel> {arr[6]};

            var actual = CollectionsHelper.Intersect(null,(new List<List<DocumentModel>> {arr1, arr2}).ToArray()).ToList();
            Assert.IsNotEmpty(actual);
        }
    }
}