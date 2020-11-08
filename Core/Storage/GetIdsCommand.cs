using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Storage
{
    public class GetIdsCommand
    {
        public Task<List<int>> GetIds(IndexModel indexModel)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}";
            var files = Directory.GetFiles(path);
            var result = 
                files
                    .Select(file =>
                    {
                        file = file.Replace('\\', '/');
                        return file.Split('/')[^1];
                    }).Select(str => str.Split('.')[0])
                    .Select(s=>Convert.ToInt32(s)).OrderBy(x=>x).ToList();


            return Task.FromResult(result);
        }
    }
}