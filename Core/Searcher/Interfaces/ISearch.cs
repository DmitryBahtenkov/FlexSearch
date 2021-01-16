using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Searcher.Interfaces
{
    public interface ISearch
    {
        public Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, BaseSearchModel searchModel);
    }
}
