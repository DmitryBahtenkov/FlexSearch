using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Core.Storage.Database;
using Newtonsoft.Json.Linq;

namespace SearchApi.Services
{
    public class DatabaseService
    {
        private string _path = $"{AppDomain.CurrentDomain.BaseDirectory}data/";
        public async Task<Guid> Insert(IndexModel model, object obj)
        {
            var id = Guid.NewGuid();
            var raw = obj.ToString();
            var document = new DocumentModel
            {
                Id = id,
                Value = JObject.Parse(raw ?? string.Empty)
            };

            using (var database = new DocumentDatabase(model))
            {
                await database.Insert(document);
            }

            return id;
        }

        public async Task Delete(IndexModel indexModel, DocumentModel documentModel)
        {
            using (var database = new DocumentDatabase(indexModel))
            {
                await database.Delete(documentModel);
            }
        }
        
        public async Task Update(IndexModel indexModel, object obj, string id)
        {
            var raw = obj.ToString();
            var document = new DocumentModel
            {
                Id = Guid.Parse(id),
                Value = JObject.Parse(raw ?? string.Empty)
            };
            using (var database = new DocumentDatabase(indexModel))
            {
                await database.Update(document);
            }
        }   
        
        public async Task<DocumentModel> FindById(IndexModel indexModel, string id)
        {
            using (var database = new DocumentDatabase(indexModel))
            {
                return await database.Find(Guid.Parse(id));
            }
        }

        public async Task DeleteDatabase(string databaseName)
        {
            await FileOperations.DeleteDirectory(_path + databaseName);
        }

        public async Task DeleteIndex(IndexModel indexModel)
        {
            await FileOperations.DeleteFile(_path + $"{indexModel}.col");
            await FileOperations.DeleteFile(_path + $"{indexModel}.pidx");
            await FileOperations.DeleteFile(_path + $"{indexModel}.sidx");
        }
    }
}