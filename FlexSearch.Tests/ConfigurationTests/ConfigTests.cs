using System.Collections.Generic;
using Core.Models;
using NUnit.Framework;

namespace GreatSearchEngineTests.ConfigurationTests
{
    public class ConfigTests
    {
        private ConfigurationModel _configurationModel;
        [SetUp]
        public void Setup()
        {
            _configurationModel = new ConfigurationModel
            {
                Host = "http://localhost",
                Port = 5000,
                Root = new RootUserModel {Password = "1234"},
                Users = new List<UserModel>(),
                Filters = new List<string>
                {
                    "LowerCase",
                    "Punctuation",
                    "Stemmer"
                },
                SyncHosts = new List<string>()
            };
        }

        [Test]
        public void ToDictionaryTest()
        {
            var dict = _configurationModel.ToDictionary();
            Assert.AreEqual(6, dict.Count);
        }
    }
}