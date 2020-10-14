using System.IO;
using System.Threading.Tasks;

namespace Storage.FileSystem
{
    public class CreateDbCommand
    {
        public void CreateDb(string name)
        {
            var path = $"/home/dmitry/Projects/GreatSearchEngine/Storage/bin/Debug/netcoreapp3.1/data/{name}";
            if(!Directory.Exists("/home/dmitry/Projects/GreatSearchEngine/Storage/bin/Debug/netcoreapp3.1/data"))
                throw new DirectoryNotFoundException("Директории data не существует");
            if (Directory.Exists(path))
                return;
            if(!Directory.CreateDirectory(path).Exists)
                throw new DirectoryNotFoundException("База данных не была создана");
        }
    }
}