using System.IO;
using System.Threading.Tasks;

namespace Storage.FileSystem
{
    public class CreateDbCommand
    {
        public void CreateDb(string name)
        {
            if(!Directory.Exists("data"))
                throw new DirectoryNotFoundException("Директории data не существует");
            if (Directory.Exists($"data/{name}"))
                return;
            if(!Directory.CreateDirectory($"data/{name}").Exists)
                throw new DirectoryNotFoundException("База данных не была создана");
        }
    }
}