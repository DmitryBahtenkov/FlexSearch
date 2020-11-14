using System;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Storage
{
    public class UpdateOperations
    {
        public async Task RenameIndex(IndexModel indexModel, string newName)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}";
            await FileOperations.RenameDirectory(path, newName);
        }
        
        public async Task RenameDatabase(IndexModel indexModel, string newName)
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel.DatabaseName}";
            await FileOperations.RenameDirectory(path, newName);
        }
    }
}