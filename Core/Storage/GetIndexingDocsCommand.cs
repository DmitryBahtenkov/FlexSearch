using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Storage
{
    public class GetIndexingDocsCommand : BaseCommand
    {
        public async Task<Dictionary<string, List<int>>> Get(string dbName, string idxName, string key)
        {
            var path = $"{AppDomain.BaseDirectory}data/{dbName}/{idxName}/indexes/{key}.json";
            var result = await ReadJsonFileCommand.ReadFile(path);
            return result.ToObject<Dictionary<string, List<int>>>();
        }
    }
}