using System;
using System.IO;
using System.Threading.Tasks;

namespace Core.Storage
{
    public class CreateIndexCommand 
    {
        public Task CreateIndex(string dbName, string indexName)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{dbName}/{indexName}";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return Task.CompletedTask;
        }
        
    }
}