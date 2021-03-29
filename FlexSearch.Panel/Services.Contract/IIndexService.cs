using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlexSearch.Panel.Models.CoreModels;
using FlexSearch.Panel.Models.ViewModels;

namespace FlexSearch.Panel.Services.Contract
{
    public interface IIndexService
    {
        public void Initialize(AuthViewModel authViewModel);
        public Task<string> Create(string dbname, string idxName, object obj);
        public Task<List<DocumentModel>> GetAll(string dbname, string idxName);
        public Task<DocumentModel> GetById(string dbname, string idxName, Guid id);
        public Task Update(string dbname, string idxName, Guid id, object obj);
        public Task DeleteItem(string dbname, string idxName, Guid id);
        public Task<string[]> GetDbs();
        public Task<string[]> GetIndexes(string dbname);
    }
}