using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage.Database;
using Newtonsoft.Json.Linq;

namespace SearchApi.Services
{
    public class DatabaseService
    {
        public async Task Insert(IndexModel model, object obj)
        {
            var raw = obj.ToString();
            var document = new DocumentModel
            {
                Id = Guid.NewGuid(),
                Value = JObject.Parse(raw ?? string.Empty)
            };

            using (var database = new DocumentDatabase(model))
            {
                await database.Insert(document);
            }
        }

        public async Task Delete(IndexModel indexModel, DocumentModel documentModel)
        {
            using (var database = new DocumentDatabase(indexModel))
            {
                await database.Delete(documentModel);
            }
        }
        
        public async Task Update(IndexModel indexModel, DocumentModel documentModel)
        {
            using (var database = new DocumentDatabase(indexModel))
            {
                await database.Update(documentModel);
            }
        }
        
        public async Task<DocumentModel> FindById(IndexModel indexModel, string id)
        {
            using (var database = new DocumentDatabase(indexModel))
            {
                return await database.Find(Guid.Parse(id));
            }
        }
    }
}