using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;

namespace Core.Searcher
{
    public class Searcher
    {
        private readonly GetIndexingDocsCommand _getIndexingDocsCommand;

        public Searcher()
        {
            _getIndexingDocsCommand = new GetIndexingDocsCommand();
        }

        private IEnumerable<DocumentModel> Intersect(ICollection<DocumentModel> firstArr,
            ICollection<DocumentModel> secondArr)
        {
            var maxLen = firstArr.Count > secondArr.Count ? firstArr.Count : secondArr.Count;

            var result = firstArr.Intersect(secondArr);

            return result;
        }

        public async Task<ICollection<DocumentModel>> Search(string dbName, string idxName, string key, string text)
        {
            var docs = await _getIndexingDocsCommand.Get(dbName, idxName, key);
            throw new NotImplementedException();
        }
    }
}