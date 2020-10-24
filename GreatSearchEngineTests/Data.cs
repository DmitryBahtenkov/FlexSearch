using System.Collections.Generic;
using Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GreatSearchEngineTests
{
    public static class Data
    {
        private static List<DocumentModel> _documents;
        public static List<DocumentModel> SetData()
        {
            _documents = new List<DocumentModel>();
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
            var five = new TestModel
            {
                Name = "Maxim",
                Text = "So, what about going to download fest 2021, but, fuck, i remembered, that damn COVID break all concerts"
            };
            var six = new TestModel
            {
                Name = "Anastasia",
                Text = "Unfortunately, I left the guitar in my second flat and parents forbade me to left my home"
            };
            var seven = new TestModel
            {
                Name = "Yandex",
                Text = "The kidnapper sent a parent mission to get Clarenbach."
            };

            var str1 = JsonConvert.SerializeObject(one);
            var str2 = JsonConvert.SerializeObject(two);
            var str3 = JsonConvert.SerializeObject(three);
            var str4 = JsonConvert.SerializeObject(four);
            var str5 = JsonConvert.SerializeObject(five);
            var str6 = JsonConvert.SerializeObject(six);
            var str7 = JsonConvert.SerializeObject(seven);
            
            _documents.Add(new DocumentModel
            {
                Id = 1,
                Value = JObject.Parse(str1)
            });
            _documents.Add(new DocumentModel
            {
                Id = 2,
                Value = JObject.Parse(str2)
            });            
            _documents.Add(new DocumentModel
            {
                Id = 3,
                Value = JObject.Parse(str3)
            });            
            _documents.Add(new DocumentModel
            {
                Id = 4,
                Value = JObject.Parse(str4)
            });
            _documents.Add(new DocumentModel
            {
                Id = 5,
                Value = JObject.Parse(str5)
            });            
            _documents.Add(new DocumentModel
            {
                Id = 6,
                Value = JObject.Parse(str6)
            });
            _documents.Add(new DocumentModel
            {
                Id = 7,
                Value = JObject.Parse(str7)
            });
            return _documents;
        }
    }
}