using Core.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Searcher.Interfaces
{
    public interface ISearch
    {
        public SearchType Type { get; }
        public Task<List<DocumentModel>> ExecuteSearch(IndexModel indexModel, SearchModel searchModel);
    }
}
