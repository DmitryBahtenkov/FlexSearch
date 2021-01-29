using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Core.Storage.Database;
using Newtonsoft.Json.Linq;

namespace SearchApi.Services
{
    public class DatabaseService 
    {
        private readonly string _path = $"{AppDomain.CurrentDomain.BaseDirectory}data/";
        private DocumentDatabase _documentDatabase;
        
        private void SetDb(IndexModel model)
        {
            if (_documentDatabase is null || _documentDatabase.Disposed)
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
            SetDb(model);
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
            SetDb(indexModel);
            await _documentDatabase.Delete(documentModel);
        }
        
        public async Task Update(IndexModel indexModel, object obj, string id)
        {
            SetDb(indexModel);
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
            SetDb(indexModel);
            return await _documentDatabase.Find(Guid.Parse(id));
        }
        
        public async Task<List<DocumentDto>> GetAll(IndexModel indexModel)
        {
            SetDb(indexModel);
            var result = await _documentDatabase.GetAllDocuments();
            return result.Select(x => 
                new DocumentDto
                {
                    Id = x.Id, 
                    Value = JsonDocument.Parse(x.Value.ToString())
                }).ToList();
        }

        public async Task DeleteDatabase(string databaseName)
        {
            await FileOperations.DeleteDirectory(_path + databaseName);
        }

        public async Task DeleteIndex(IndexModel indexModel)
        {
            SetDb(indexModel);
            _documentDatabase.Dispose();
            await FileOperations.DeleteFile(_path + $"{indexModel}.col");
            await FileOperations.DeleteFile(_path + $"{indexModel}.pidx");
            await FileOperations.DeleteFile(_path + $"{indexModel}.sidx");
        }
        
        public Task<List<string>> GetDatabases()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/";
            FileOperations.CheckOrCreateDirectory(path);
            var dbs = Directory.GetDirectories(path).ToList();
            var result = dbs.Select(db => db.Replace('\\', '/').Split("/").LastOrDefault()).ToList();

            return Task.FromResult(result);
        }
        
        public async Task RenameIndex(IndexModel indexModel, string newName)
        {
            SetDb(indexModel);
            _documentDatabase.Dispose();
            await FileOperations.RenameFile(_path + $"{indexModel}.col", $"{newName}.col");
            await FileOperations.RenameFile(_path + $"{indexModel}.pidx", $"{newName}.pidx");
            await FileOperations.RenameFile(_path + $"{indexModel}.sidx", $"{newName}.sidx");
        }
    }
}