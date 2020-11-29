using System;
using System.Threading.Tasks;
using Core.Models;

namespace Core.Storage
{
    public static class DeleteOperations
    {
        public static async Task DeleteIndex(IndexModel indexModel)
        {
            await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel}");
        }
        public static async Task DeleteDatabase(string dbname)
        {
            await FileOperations.DeleteDirectory($"{AppDomain.CurrentDomain.BaseDirectory}data/{dbname}");
        }
    }
}