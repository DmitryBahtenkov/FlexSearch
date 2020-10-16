using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Storage
{
    public class ReadJsonFileCommand
    {
        public static async Task<JObject> ReadFile(string path)
        {
            using var sr = new StreamReader(path);
            var obj = await sr.ReadToEndAsync();

            return JsonConvert.DeserializeObject<JObject>(obj);
        }
    }
}