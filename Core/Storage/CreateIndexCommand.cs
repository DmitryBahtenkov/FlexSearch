using System.IO;

namespace Core.Storage
{
    public class CreateIndexCommand : BaseCommand
    {
        private readonly CreateDbCommand _createDbCommand;

        public CreateIndexCommand()
        {
            _createDbCommand = new CreateDbCommand();
        }
        public void CreateIndex(string dbName, string name)
        {
            var path = $"{AppDomain.BaseDirectory}data/{dbName}"; 
            if(!Directory.Exists(path))
                _createDbCommand.CreateDb(dbName);
            path += $"/{name}";
            var index = Directory.CreateDirectory(path);
            if(!index.Exists)
                throw new DirectoryNotFoundException("Индекс не был создан");
        }
    }
}