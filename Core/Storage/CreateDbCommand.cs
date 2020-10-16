using System.IO;

namespace Core.Storage
{
    public class CreateDbCommand : BaseCommand
    {
        public void CreateDb(string name)
        {
            var path = $"{AppDomain.BaseDirectory}data/{name}";
            if(!Directory.Exists($"{AppDomain.BaseDirectory}/data"))
                throw new DirectoryNotFoundException("Директории data не существует");
            if (Directory.Exists(path))
                return;
            if(!Directory.CreateDirectory(path).Exists)
                throw new DirectoryNotFoundException("База данных не была создана");
        }
    }
}