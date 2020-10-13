using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.FileSystem
{
    public class GetIdsCommand
    {
        public static Task<List<int>> GetIds(string dbName, string indexName)
        {
            var path = $"data/{dbName}/{indexName}";
            var files = Directory.GetFiles(path);
            var result = 
                files
                    .Select(file => file.Split('/')[^1]).Select(str => str.Split('.')[0])
                    .Select(s=>Convert.ToInt32(s)).OrderBy(x=>x).ToList();


            return Task.FromResult(result);
        }
    }
}