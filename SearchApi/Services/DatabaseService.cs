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
        private DocumentDatabase _documentDatabase;
        
        private void SetDb(IndexModel model)
        {
            if (_documentDatabase is null)
            {
                _documentDatabase = new DocumentDatabase(model);
                return;
            }
            if (_documentDatabase?.IndexModel != model)
            {
                _documentDatabase = new DocumentDatabase(model);
            }
        }
        
        
        public async Task<Guid> Insert(IndexModel model, object obj)
        {
            
            var id = Guid.NewGuid();
            var raw = obj.ToString();
            var document = new DocumentModel
            {
                Id = id,
                Value = JObject.Parse(raw ?? string.Empty)
            };
            
            await _documentDatabase.Insert(document);


            return id;
        }

        public async Task Delete(IndexModel indexModel, DocumentModel documentModel)
        {
            await _documentDatabase.Delete(documentModel);
        }
        
        public async Task Update(IndexModel indexModel, object obj, string id)
        {
            var raw = obj.ToString();
            var document = new DocumentModel
            {
                Id = Guid.Parse(id),
                Value = JObject.Parse(raw ?? string.Empty)
            };
            await _documentDatabase.Update(document);
        }   
        
        public async Task<DocumentModel> FindById(IndexModel indexModel, string id)
        {
            await _documentDatabase.Find(Guid.Parse(id));
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