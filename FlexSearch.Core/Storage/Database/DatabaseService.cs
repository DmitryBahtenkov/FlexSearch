using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Newtonsoft.Json.Linq;

namespace Core.Storage.Database
{
    public static class DatabaseService 
    {
        private static readonly string Path = $"{AppDomain.CurrentDomain.BaseDirectory}data/";
        private static DocumentDatabase _documentDatabase;
        
        private static  void SetDb(IndexModel model)
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
        
        
        public static async Task<Guid> Insert(IndexModel model, object obj)
        {
            //SetDb(model);
            var id = Guid.NewGuid();
            var raw = obj.ToString();
            var document = new DocumentModel
            {
                Id = id,
                Value = JObject.Parse(raw ?? string.Empty)
            };
            using (var db = new DocumentDatabase(model))
            {
                await db.Insert(document);
            }
            
            return id;
        }

        public static  async Task Delete(IndexModel indexModel, DocumentModel documentModel)
        {
            //SetDb(indexModel);
            using (var db = new DocumentDatabase(indexModel))
            {
                await db.Delete(documentModel);
            }
        }
        
        public static async Task<List<Dictionary<string, Guid>>> GetIndexes(IndexModel indexModel, string key)
        {
            //SetDb(indexModel);
            using (var db = new DocumentDatabase(indexModel))
            {
                return await db.GetIndexes(key);
            }
        }
        
        public static  async Task Update(IndexModel indexModel, object obj, string id)
        {
            //SetDb(indexModel);
            var raw = obj.ToString();
            var document = new DocumentModel
            {
                Id = Guid.Parse(id),
                Value = JObject.Parse(raw ?? string.Empty)
            };

            using (var db = new DocumentDatabase(indexModel))
            {
                await db.Update(document);

            }
        }   
        
        public static  async Task<DocumentModel> FindById(IndexModel indexModel, Guid id)
        {
            using (var db = new DocumentDatabase(indexModel))
            {
                return await db.Find(id);
            }
        }
        
        public static  async Task<List<DocumentModel>> GetAll(IndexModel indexModel)
        {
            //SetDb(indexModel);
            using (var db = new DocumentDatabase(indexModel))
            {
                return await db.GetAllDocuments();
            }
        }

        public static async Task DeleteDatabase(string databaseName)
        {
            await FileOperations.DeleteDirectory(Path + databaseName);
        }

        public static async Task DeleteIndex(IndexModel indexModel)
        {
            //SetDb(indexModel);
            //_documentDatabase.Dispose();
            await FileOperations.DeleteFile(Path + $"{indexModel}.col");
            await FileOperations.DeleteFile(Path + $"{indexModel}.pidx");
            await FileOperations.DeleteFile(Path + $"{indexModel}.sidx");
        }
        
        public static Task<List<string>> GetDatabases()
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/";
            FileOperations.CheckOrCreateDirectory(path);
            var dbs = Directory.GetDirectories(path).ToList();
            var result = dbs.Select(db => db.Replace('\\', '/').Split("/").LastOrDefault()).ToList();

            return Task.FromResult(result);
        }
        
        public static async Task RenameIndex(IndexModel indexModel, string newName)
        {
            //SetDb(indexModel);
            //_documentDatabase.Dispose();
            await FileOperations.RenameFile(Path + $"{indexModel}.col", $"{newName}.col");
            await FileOperations.RenameFile(Path + $"{indexModel}.pidx", $"{newName}.pidx");
            await FileOperations.RenameFile(Path + $"{indexModel}.sidx", $"{newName}.sidx");
        }
        
        public static Task<List<string>> GetIndexes(string dbname)
        {
            var files = Directory.GetFiles(Path + dbname).Select(x=>x.Replace("\\", "/"));
            var result = new List<string>();

            foreach (var file in files)
            {
                var f = file.Split(dbname+ "/").LastOrDefault()?.Split(".").FirstOrDefault();

                if (!result.Contains(f))
                {
                    result.Add(f);
                }
            }
            
            return Task.FromResult(result);
        }
    }
}