using System;
using System.Threading.Tasks;
using Core.Storage;

namespace SearchApi.System
{
    public class CoreFacade
    {
        private readonly CreateIndexCommand _createIndexCommand;
        private readonly AddObjectToIndexCommand _addObjectToIndexCommand;
        private readonly IndexingDocumentsCommand _indexingDocumentsCommand;
        private readonly GetDocumentsCommand _getDocumentsCommand;

        public CoreFacade()
        {
            _createIndexCommand = new CreateIndexCommand();
            _addObjectToIndexCommand = new AddObjectToIndexCommand();
            _indexingDocumentsCommand = new IndexingDocumentsCommand();
            _getDocumentsCommand = new GetDocumentsCommand();
        }

        public async Task<bool> CreateAll(string dbname, string indexName, object obj)
        {
            var raw = obj.ToString();
            try
            {
                await _createIndexCommand.CreateIndex(dbname, indexName);
                await _addObjectToIndexCommand.Add(dbname, indexName, raw);
                await _indexingDocumentsCommand.Indexing(dbname, indexName);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}