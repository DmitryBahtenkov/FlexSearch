using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Newtonsoft.Json;

namespace Core.Storage
{
    public class WriteJsonFileCommand
    {
        public static async Task WriteFile(string path, DocumentModel obj)
        {
            await using var sw = new StreamWriter(path);
            var str = JsonConvert.SerializeObject(obj);
            await sw.WriteAsync(str);
        }
    }
}