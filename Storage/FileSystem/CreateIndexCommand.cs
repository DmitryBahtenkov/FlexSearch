using System.IO;
using System.Threading.Tasks;
using Analyzer.Models;

namespace Storage.FileSystem
{
    public class CreateIndexCommand
    {
        private readonly CreateDbCommand _createDbCommand;
        private readonly WriteJsonFileCommand _fileCommand;

        public CreateIndexCommand()
        {
            _createDbCommand = new CreateDbCommand(); 
            _fileCommand = new WriteJsonFileCommand();
        }
        public void CreateIndex(string dbName, string name)
        {
            var path = $"data/{dbName}"; 
            if(!Directory.Exists(path))
                _createDbCommand.CreateDb(dbName);
            path += $"/{name}";
            var index = Directory.CreateDirectory(path);
            if(!index.Exists)
                throw new DirectoryNotFoundException("Индекс не был создан");
        }
    }
}