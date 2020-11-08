using System;
using System.IO;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Storage
{
    public class CreateIndexCommand 
    {
        public Task CreateIndex(IndexModel indexModel)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return Task.CompletedTask;
        }
        
    }
}